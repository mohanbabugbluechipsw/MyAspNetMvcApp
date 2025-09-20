

using BLL.IService;
using DAL.IRepositories;
using Microsoft.AspNetCore.Http;
using Model_New.Models;

using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Text.Encodings.Web;


namespace BLL.Services
{
    public class ErrorService : IErrorService
    {
        private readonly IErrorRepository _errorRepository;
        private readonly IEmailService _emailService;

        public ErrorService(IErrorRepository errorRepository, IEmailService emailService)
        {
            _errorRepository = errorRepository;
            _emailService = emailService;
        }







        public async Task<Guid> HandleExceptionAsync(Exception exception, HttpContext context)
        {
            var errorId = Guid.NewGuid();
            var user = context.User;
            var username = user.Identity?.IsAuthenticated == true ? user.Identity.Name : "Anonymous";

            var errorLog = new ApplicationErrorLog
            {
                ErrorLogId = errorId,
                ErrorMessage = exception.Message,
                StackTrace = exception.StackTrace,
                Endpoint = context.Request.Path,
                LoggedAt = DateTime.UtcNow,
                HttpMethod = context.Request.Method,
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                UserIP = context.Connection.RemoteIpAddress?.ToString(),
                Status = "New",
                RSCode = context.Items.ContainsKey("RSCode") ? context.Items["RSCode"]?.ToString() : null,
                OutletCode = context.Items.ContainsKey("OutletCode") ? context.Items["OutletCode"]?.ToString() : null,
                ReviewDetailsJson = context.Items.ContainsKey("ReviewContext")
                    ? JsonConvert.SerializeObject(context.Items["ReviewContext"])
                    : null,
                Username = username
            };

            try
            {
                await _errorRepository.AddErrorAsync(errorLog);
            }
            catch (Exception dbEx)
            {
                Console.WriteLine($"[ErrorService] DB save failed: {dbEx.Message}");
            }

            DateTime indiaTime;
            try
            {
                var istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                indiaTime = TimeZoneInfo.ConvertTimeFromUtc(errorLog.LoggedAt, istTimeZone);
            }
            catch
            {
                indiaTime = errorLog.LoggedAt;
            }

            var subject = "Merchandizer Application System Error Log – Error Notification";
            var body = $@"
<html>
<head>
    <style>
        table {{ border-collapse: collapse; width: 100%; font-family: Arial; }}
        th, td {{ border: 1px solid #ddd; padding: 8px; }}
        th {{ background-color: #f2f2f2; }}
    </style>
</head>
<body>
    <h2>Merchandizer Application System Error Notification</h2>
    <p><strong>User:</strong> {WebUtility.HtmlEncode(username)}</p>
    <table>
        <tr><th>Error ID</th><td>{errorLog.ErrorLogId}</td></tr>
        <tr><th>Error Type</th><td>{exception.GetType().Name}</td></tr>
        <tr><th>Occurred At (IST)</th><td>{indiaTime:yyyy-MM-dd HH:mm:ss}</td></tr>
        <tr><th>Endpoint</th><td>{WebUtility.HtmlEncode(errorLog.Endpoint)}</td></tr>
        <tr><th>HTTP Method</th><td>{errorLog.HttpMethod}</td></tr>
        <tr><th>User Agent</th><td>{WebUtility.HtmlEncode(errorLog.UserAgent)}</td></tr>
        <tr><th>User IP</th><td>{errorLog.UserIP}</td></tr>
        <tr><th>RSCode</th><td>{errorLog.RSCode ?? "N/A"}</td></tr>
        <tr><th>OutletCode</th><td>{errorLog.OutletCode ?? "N/A"}</td></tr>
        <tr><th>Error Message</th><td>{WebUtility.HtmlEncode(errorLog.ErrorMessage)}</td></tr>
    </table>";

            if (!string.IsNullOrWhiteSpace(errorLog.ReviewDetailsJson))
            {
                try
                {
                    var reviewDetails = JsonConvert.DeserializeObject<Dictionary<string, object>>(errorLog.ReviewDetailsJson);
                    body += "<h3>Review Context:</h3><table>";

                    foreach (var kvp in reviewDetails)
                    {
                        body += $"<tr><th>{WebUtility.HtmlEncode(kvp.Key)}</th><td>{WebUtility.HtmlEncode(kvp.Value?.ToString() ?? "N/A")}</td></tr>";
                    }

                    body += "</table>";
                }
                catch
                {
                    body += $@"
            <h3>Review Context (Raw JSON):</h3>
            <pre>{WebUtility.HtmlEncode(errorLog.ReviewDetailsJson)}</pre>";
                }
            }

            body += @"
    <p>Please review and take necessary action.</p>
    <p>Thank you,<br/>Azure Support Team</p>
</body>
</html>";

            try
            {
                await _emailService.SendErrorNotificationAsync(errorLog, subject, body, isHighPriority: true);
            }
            catch (Exception emailEx)
            {
                Console.WriteLine($"[ErrorService] Email sending failed: {emailEx.Message}");
            }

            return errorLog.ErrorLogId;
        }



        // Resolve Error and Send Resolution Notification
        public async Task<bool> ResolveErrorAsync(Guid errorId)
        {
            var errorLog = await _errorRepository.GetErrorByIdAsync(errorId);

            if (errorLog == null)
                return false;

            errorLog.Status = "Resolved";
            await _errorRepository.UpdateErrorAsync(errorLog);

            var resolvedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time"));

            var subject = "Merchandizer Application System – Issue Resolved";

            var body = $@"
                <html>
                <body>
                    <h2>Issue Resolved Notification</h2>
                    <p>The issue with Error ID: {errorLog.ErrorLogId} has been successfully resolved.</p>
                    <table border='1' cellpadding='5' cellspacing='0'>
                        <tr><th>Endpoint</th><td>{errorLog.Endpoint}</td></tr>
                        <tr><th>Resolved At</th><td>{resolvedTime:yyyy-MM-dd HH:mm:ss}</td></tr>
                    </table>
                    <p>Thank you,<br/>Support Team</p>
                </body>
                </html>";

            await _emailService.SendResolutionNotificationAsync(errorLog, subject, body);

            return true;
        }


        public async Task<bool> UpdateErrorStatusAsync(Guid errorId, string status)
        {
            var errorLog = await _errorRepository.GetErrorByIdAsync(errorId);

            if (errorLog == null)
                return false;

            errorLog.Status = status;

            await _errorRepository.UpdateErrorAsync(errorLog);

            // If resolved, send resolution email
            if (status.Equals("Resolved", StringComparison.OrdinalIgnoreCase))
            {
                var subject = "Resolution: Merchandizer Application System Error Resolved";
                var body = $"Error ID: {errorLog.ErrorLogId} has been resolved successfully.";

                await _emailService.SendResolutionNotificationAsync(errorLog, subject, body);
            }

            return true;
        }



      
    }
}
