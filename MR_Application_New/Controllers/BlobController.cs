


using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Data.SqlClient;
using System.Security.Claims;

namespace MR_Application_New.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly ILogger<BlobController> _logger;
        private readonly RecyclableMemoryStreamManager _memoryStreamManager;
        private readonly IConfiguration _configuration;


        public BlobController(IConfiguration config, BlobServiceClient blobServiceClient, ILogger<BlobController> logger, RecyclableMemoryStreamManager memoryStreamManager)
        {
            _blobServiceClient = blobServiceClient;
            _accountName = config["AzureBlob:AccountName"];
            _accountKey = config["AzureBlob:AccountKey"];
            _logger = logger;
            _memoryStreamManager = memoryStreamManager;

            _configuration = config; // ✅ store config for DB


        }

        private async Task<BlobContainerClient> GetContainerClientAsync(string container)
        {
            var client = _blobServiceClient.GetBlobContainerClient(container);
            await client.CreateIfNotExistsAsync(PublicAccessType.Blob);
            return client;
        }

        private async Task HandleErrorAsync(Exception ex, string actionName, Guid? reviewId = null)
        {
            var controllerName = ControllerContext.ActionDescriptor.ControllerName;

            _logger.LogError(ex, "Error in {Controller}/{Action}", controllerName, actionName);

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var errorSql = @"
        INSERT INTO tbl_ErrorLog (
            ControllerName, ActionName, ErrorCode, ErrorMessage, StackTrace,
            ReviewId, ServerName, ApplicationName, CreatedAt
        )
        VALUES (
            @ControllerName, @ActionName, @ErrorCode, @ErrorMessage, @StackTrace,
            @ReviewId, @ServerName, @ApplicationName, SYSDATETIMEOFFSET() AT TIME ZONE 'India Standard Time'
        );";

            await connection.ExecuteAsync(errorSql, new
            {
                ControllerName = controllerName,
                ActionName = actionName,
                ErrorCode = ex.HResult,
                ErrorMessage = ex.Message,
                StackTrace = ex.ToString(),
                ReviewId = reviewId,
                ServerName = Environment.MachineName,
                ApplicationName = "MR_Application_New"
            });
        }


        private static string GetTodayFolderName()
        {
            var ist = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ist);
            return now.ToString("yyyyMMdd");
        }

        private IActionResult LogAndReturnError(Exception ex, object? extraInfo = null)
        {
            var log = new
            {
                AppErrorId = Guid.NewGuid(),
                Message = ex.Message,
                StackTrace = ex.ToString(),
                Endpoint = HttpContext?.Request?.Path.Value,
                OccurredAt = DateTime.UtcNow,
                HttpMethod = HttpContext?.Request?.Method,
                UserAgent = Request.Headers["User-Agent"].ToString(),
                UserIP = HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                Status = "500",
                ResolvedAt = (DateTime?)null,
                Username = User?.Identity?.Name,
                UserRoles = string.Join(",", User?.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value) ?? new List<string>()),
                RSCode = "", // optional: can be filled dynamically from your flow
                OutletCode = "",
                ReviewDetailsJson = System.Text.Json.JsonSerializer.Serialize(extraInfo ?? new { })
            };

            _logger.LogError(ex, "AppErrorId: {AppErrorId} | Endpoint: {Endpoint} | User: {User}",
                log.AppErrorId, log.Endpoint, log.Username ?? "Anonymous");

            return StatusCode(500, new { errorId = log.AppErrorId, error = "Something went wrong. Please contact support with the errorId." });
        }

        // ✅ SAS generator
        [HttpGet("GetUploadSas")]
        [RequestSizeLimit(1_700 * 1024 * 1024)]
        public async Task <IActionResult> GetUploadSas(string fileName, string container = "reviewphoto")
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("File name is required.");

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(fileName);

                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = container,
                    BlobName = fileName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

                var sasToken = sasBuilder.ToSasQueryParameters(
                    new StorageSharedKeyCredential(_accountName, _accountKey)).ToString();

                return Ok(new
                {
                    uploadUrl = $"{blobClient.Uri}?{sasToken}",
                    viewUrl = blobClient.Uri.ToString()
                });
            }
            catch (Exception ex)
            {

                await HandleErrorAsync(ex, nameof(GetUploadSas));

                return LogAndReturnError(ex, new { FileName = fileName, Container = container });
            }
        }

        // ✅ Optimized Upload Login Photo
        [AllowAnonymous]
        [HttpPost("UploadLoginPhoto")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(1_700 * 1024 * 1024)]
        public async Task<IActionResult> UploadLoginPhoto([FromForm] IFormFile file, string container = "loginphoto")
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file provided." });

            try
            {
                using var image = await Image.LoadAsync(file.OpenReadStream());

                if (image.Width > 800 || image.Height > 800)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(800, 800)
                    }));
                }

                await using var ms = _memoryStreamManager.GetStream();
                await image.SaveAsJpegAsync(ms, new JpegEncoder { Quality = 70 });

                if (ms.Length > 2_000_000)
                    return BadRequest(new { error = "Compressed image still exceeds 2 MB limit." });

                ms.Position = 0;

                var containerClient = await GetContainerClientAsync(container);

                var folder = GetTodayFolderName();
                var uniqueName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}.jpg";
                var blobName = $"{folder}/{uniqueName}";

                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.UploadAsync(ms, new BlobHttpHeaders { ContentType = "image/jpeg" });

                return Ok(new
                {
                    url = blobClient.Uri.ToString(),
                    fileName = uniqueName,
                    blobName,
                    container
                });
            }
            catch (Exception ex)
            {

                await HandleErrorAsync(ex, nameof(UploadLoginPhoto));

                return LogAndReturnError(ex, new { File = file?.FileName, Container = container });
            }
        }
    }
}
