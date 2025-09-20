

using Azure.Storage;
using Azure.Storage.Blobs;
using BLL;
using BLL.IService;
using BLL.Services;
using DAL;
using DAL.IRepositories;
using DAL.Repositories;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IO;
using Model_New.Configuration;
using Model_New.Models;
using MR_Application_New.Filters.Action;
using MR_Application_New.Filters.Authorization;
using MR_Application_New.Filters.Result;
using Serilog;
using Serilog.Events;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
builder.Host.UseSerilog((context, services, config) =>
{
    config.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
          .MinimumLevel.Debug()
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);
});

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "MRApp.Auth";
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Login/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(6);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Database
builder.Services.AddDbContext<MrAppDbNewContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromMinutes(60), null);
            sqlOptions.CommandTimeout(1800);
        }));



// Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new Hangfire.SqlServer.SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(60),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(60),
            QueuePollInterval = TimeSpan.FromSeconds(15),
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));
builder.Services.AddHangfireServer();

// Services & Repositories
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IErrorRepository, ErrorRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IErrorService, ErrorService>();
builder.Services.AddScoped<IMRDataService, MRDataService>();
builder.Services.AddScoped<IOSAReviewRepository, OSAReviewRepository>();
builder.Services.AddScoped<IOSAReviewService, OSAReviewService>();
builder.Services.AddScoped<IOSAServiceRepository, OSAService>();
builder.Services.AddScoped<IGetDailydetailsummaryReportRepository, GetDailydetailsummaryReportRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IOutletMRSearchService, OutletMRSearchService>();
builder.Services.AddScoped<IOutletMRSearchRepository, OutletMRSearchRepository>();
builder.Services.AddScoped(typeof(UnitOfWork<MrAppDbNewContext>));
builder.Services.AddScoped<IReminderJobService, ReminderJobService>();
builder.Services.AddScoped<IStoreVisitQuestionRepository, StoreVisitQuestionRepository>();
builder.Services.AddScoped<IReviewContextAccessor, ReviewContextAccessor>();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IVisitorReviewReportRepository, VisitorReviewReportRepository>();
builder.Services.AddScoped<IVisitorReviewReportJob, VisitorReviewReportJob>();


string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddScoped<IStoreVisitRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new StoreVisitRepository(connectionString, configuration);
});

builder.Services.AddScoped<IStoreVisitMailer, StoreVisitMailer>();


builder.Services.AddScoped<IStoreReviewEmailService>(sp =>
    new AzureEmailService(
        builder.Configuration["AzureCommunicationServices:ConnectionString"],
        builder.Configuration["AzureCommunicationServices:SenderAddress"]
    ));

// Filters
builder.Services.AddScoped<LogActionFilter>();
builder.Services.AddScoped<CustomResultFilter>();

// Session & File Uploads
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(5);
    options.Cookie.Name = "MRApp.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


builder.Services.AddSingleton(x =>
{
    var accountName = builder.Configuration["AzureBlob:AccountName"];
    var accountKey = builder.Configuration["AzureBlob:AccountKey"];

    return new BlobServiceClient(
        new Uri($"https://{accountName}.blob.core.windows.net"),
        new StorageSharedKeyCredential(accountName, accountKey));
});


builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1_700_000_000; // 1.7 GB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 1_700_000_000;
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(60);
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// MVC + JSON
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton<RecyclableMemoryStreamManager>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<MrAppDbNewContext>(name: "database", failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready" });

// Other configs
builder.Services.AddHttpClient(); // For warmup

// Validate Config (non-fatal)
var acsConfig = builder.Configuration.GetSection("AzureCommunicationServices");
var emailConfig = builder.Configuration.GetSection("EmailSettings");

if (string.IsNullOrWhiteSpace(acsConfig["ConnectionString"]))
    Log.Warning("Azure Communication Services connection string is missing.");
if (string.IsNullOrWhiteSpace(acsConfig["SenderAddress"]))
    Log.Warning("Azure Communication Services sender address is missing.");
if (string.IsNullOrWhiteSpace(emailConfig["RecipientAddress"]))
    Log.Warning("Email recipient address is missing.");

var app = builder.Build();

// Middleware
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Redirects to Error action

    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

    app.UseHsts();
}


app.UseSerilogRequestLogging();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/favicon.ico")
    {
        context.Response.StatusCode = 204; // No Content
        return;
    }
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Hangfire dashboard

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorization() }
});


// Status Code Handler
//app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

// Endpoints
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
}).WithMetadata(new AllowAnonymousAttribute());

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
}).WithMetadata(new AllowAnonymousAttribute());

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready")
}).WithMetadata(new AllowAnonymousAttribute());

app.MapGet("/warmup", async (HttpContext ctx, IHttpClientFactory httpClientFactory) =>
{
    _ = Task.Run(async () =>
    {
        try
        {
            var baseUrl = $"{(ctx.Request.IsHttps ? "https" : "http")}://{ctx.Request.Host}";
            var client = httpClientFactory.CreateClient();
            await client.GetAsync($"{baseUrl}/Login/Index");
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Warmup background hit to front page failed");
        }
    });

    return Results.Ok("Warmup OK");
}).WithMetadata(new AllowAnonymousAttribute());




app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var recurring = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurring.AddOrUpdate<StoreVisitMailer>(
        "DailyStoreVisitReportJob_6PM",
        mailer => mailer.SendDailyReports(),
        "0 18 * * *",  // every day at 6 PM
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
        });
});


















app.Run();

