

using BLL.IService;
using DAL.IRepositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model_New.Configuration;
using Model_New.ViewModels;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace BLL.Services
{
    public class StoreVisitMailer : IStoreVisitMailer
    {
        private readonly IStoreVisitRepository _repository;
        private readonly IStoreReviewEmailService _emailService;
        private readonly EmailSettings _emailSettings;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StoreVisitMailer> _logger;

        public StoreVisitMailer(
            IStoreVisitRepository repository,
            IStoreReviewEmailService emailService,
            IOptions<EmailSettings> emailOptions,
            IConfiguration configuration,
            ILogger<StoreVisitMailer> logger)
        {
            _repository = repository;
            _emailService = emailService;
            _emailSettings = emailOptions.Value;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendDailyReports()
        {
            try
            {
                await _repository.RunInsertProcedureAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "RunInsertProcedureAsync");
                return;
            }

            IEnumerable<StoreVisitLogDto> data;
            try
            {
                data = await _repository.GetTodayVisitLogsAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "GetTodayVisitLogsAsync");
                return;
            }

            var grouped = data.GroupBy(x => new { x.Rscode, x.Rs_Email });

            var emailLogs = new List<(string Rscode, string Recipient, string Status, string ErrorMessage)>();

            foreach (var group in grouped)
            {
                string rscode = group.Key.Rscode;
                string rsEmail = group.Key.Rs_Email;

                string csvContent;
                string htmlBody;
                try
                {
                    csvContent = BuildCsv(group);
                    htmlBody = BuildHtmlBody(group);
                }
                catch (Exception ex)
                {
                    emailLogs.Add((rscode, rsEmail, "Failed", $"Build Error: {ex.Message}"));
                    continue;
                }

                try
                {
                    var ccList = _emailSettings.VisitorReportCcAddress
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(e => e.Trim())
                        .ToList();

                    await _emailService.SendEmailAsync(
                        to: rsEmail,
                        cc: string.Join(',', ccList),
                        subject: $"Visitor Review Report- {rscode} ({DateTime.Now:yyyy-MM-dd})",
                        body: htmlBody,
                        csvContent: csvContent,
                        fileName: $"StoreVisit_{rscode}_{DateTime.Now:yyyyMMdd}.csv"
                    );

                    emailLogs.Add((rscode, rsEmail, "Success", null));
                }
                catch (Exception ex)
                {
                    emailLogs.Add((rscode, rsEmail, "Failed", ex.Message));
                }
            }

            // Insert logs
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();
                var reportDate = DateTime.Today;

                foreach (var log in emailLogs)
                {
                    var sql = @"
                        INSERT INTO tbl_Email_Daily_Summary_Log
                        (Rscode, RecipientEmail, Status, ErrorMessage, SentAt, ReportDate, CreatedBy)
                        VALUES (@Rscode, @RecipientEmail, @Status, @ErrorMessage,
                        SYSDATETIMEOFFSET() AT TIME ZONE 'India Standard Time', @ReportDate, 'System');";

                    await connection.ExecuteAsync(sql, new
                    {
                        Rscode = log.Rscode,
                        RecipientEmail = log.Recipient,
                        Status = log.Status,
                        ErrorMessage = log.ErrorMessage,
                        ReportDate = reportDate
                    });
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "InsertEmailLogs");
            }

            // Send summary email
            try
            {
                var summaryBody = new StringBuilder();
                summaryBody.AppendLine("<p>Daily Store Visit Email Summary:</p>");
                summaryBody.AppendLine("<table border='1' cellpadding='5'><tr><th>RS Code</th><th>Recipient</th><th>Status</th><th>Error</th></tr>");
                foreach (var log in emailLogs)
                {
                    summaryBody.AppendLine(
                        $"<tr><td>{log.Rscode}</td><td>{log.Recipient}</td><td>{log.Status}</td><td>{WebUtility.HtmlEncode(log.ErrorMessage ?? "")}</td></tr>");
                }
                summaryBody.AppendLine("</table>");

                // Build CSV for summary too
                var summaryCsv = BuildSummaryCsv(emailLogs);

                var adminEmails = _emailSettings.VisitorReportCcAddress
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .ToList();

                await _emailService.SendEmailAsync(
    to: _emailSettings.VisitorReportCcAddress,  // pass as-is
    cc: null,
    subject: $"[Summary] Daily Store Visit Emails - {DateTime.Now:yyyy-MM-dd}",
    body: summaryBody.ToString(),
    csvContent: summaryCsv,
    fileName: $"Summary_{DateTime.Now:yyyyMMdd}.csv"
);

            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "SendSummaryEmail");
            }
        }

        private string BuildCsv(IEnumerable<StoreVisitLogDto> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Rscode,RS_Name,MRNo,MRName,SrCode,SrName,OutletCode,RouteName,ChildParty,OutletName,ServicingPLG,ReviewStartTime,ReviewEndTime,Status,TimeSpent,TargetDate,SendDateTime");

            foreach (var r in rows)
            {
                sb.AppendLine($"{r.Rscode},{r.RS_Name},{r.MRNo},{r.MRName},{r.SrCode},{r.SrName},{r.OutletCode},{r.RouteName},{r.ChildParty},{r.OutletName},{r.ServicingPLG},{r.ReviewStartTime},{r.ReviewEndTime},{r.Status},{r.TimeSpent},{r.TargetDate:yyyy-MM-dd},{r.SendDateTime?.ToString("yyyy-MM-dd HH:mm") ?? "NULL"}");
            }

            return sb.ToString();
        }

        private string BuildSummaryCsv(IEnumerable<(string Rscode, string Recipient, string Status, string ErrorMessage)> logs)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Rscode,Recipient,Status,ErrorMessage");
            foreach (var log in logs)
            {
                sb.AppendLine($"{log.Rscode},{log.Recipient},{log.Status},{log.ErrorMessage}");
            }
            return sb.ToString();
        }

        private string BuildHtmlBody(IEnumerable<StoreVisitLogDto> reports)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<p>Dear Account Manager / Area Manager,</p>");
            sb.AppendLine("<p>For your reference, please find the attached report and the details below:</p>");
            sb.AppendLine("<table style='border-collapse: collapse; width: 100%;' border='1' cellpadding='5'>");
            sb.AppendLine("<tr style='background-color: #f2f2f2;'><th>Rscode</th><th>MR Name</th><th>MR No</th><th>SR Name</th><th>Outlet</th><th>Start</th><th>End</th><th>Status</th><th>Time Spent</th></tr>");

            foreach (var r in reports)
            {
                sb.AppendLine(
                    $"<tr><td>{WebUtility.HtmlEncode(r.Rscode)}</td><td>{WebUtility.HtmlEncode(r.MRName)}</td><td>{WebUtility.HtmlEncode(r.MRNo)}</td><td>{WebUtility.HtmlEncode(r.SrName)}</td><td>{WebUtility.HtmlEncode(r.OutletName)}</td><td>{WebUtility.HtmlEncode(r.ReviewStartTime)}</td><td>{WebUtility.HtmlEncode(r.ReviewEndTime)}</td><td>{WebUtility.HtmlEncode(r.Status)}</td><td>{WebUtility.HtmlEncode(r.TimeSpent)}</td></tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine("<br/><p>Thank you,<br/>Azure Support Team</p>");
            return sb.ToString();
        }

        private async Task HandleErrorAsync(Exception ex, string actionName, Guid? reviewId = null)
        {
            var controllerName = "StoreVisitMailer";
            _logger.LogError(ex, "Error in {Controller}/{Action}", controllerName, actionName);

            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();
                var errorSql = @"
                    INSERT INTO tbl_ErrorLog (
                        ControllerName, ActionName, ErrorCode, ErrorMessage, StackTrace,
                        ReviewId, ServerName, ApplicationName, CreatedAt
                    )
                    VALUES (
                        @ControllerName, @ActionName, @ErrorCode, @ErrorMessage, @StackTrace,
                        @ReviewId, @ServerName, @ApplicationName,
                        SYSDATETIMEOFFSET() AT TIME ZONE 'India Standard Time');";

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
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Failed to log error in tbl_ErrorLog");
            }

            try
            {
                var adminEmails = _emailSettings.VisitorReportCcAddress
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .ToList();

                string subject = $"[ERROR] {controllerName} - {actionName} failed";
                string body = $@"
                    <p>An error occurred in <b>{controllerName}/{actionName}</b>:</p>
                    <p><b>Message:</b> {WebUtility.HtmlEncode(ex.Message)}</p>
                    <p><b>Stack Trace:</b><br/>{WebUtility.HtmlEncode(ex.ToString()).Replace(Environment.NewLine, "<br/>")}</p>
                    <p>Server: {Environment.MachineName}</p>
                    <p>Application: MR_Application_New</p>";

                // Attach error as CSV
                var errorCsv = $"ErrorCode,Message\n{ex.HResult},\"{ex.Message.Replace("\"", "\"\"")}\"";

                //await _emailService.SendEmailAsync(
                //    to: string.Join(",", adminEmails),
                //    cc: null,
                //    subject: subject,
                //    body: body,
                //    csvContent: errorCsv,
                //    fileName: $"Error_{DateTime.Now:yyyyMMddHHmmss}.csv"
                //);

                await _emailService.SendEmailAsync(
    to: _emailSettings.VisitorReportCcAddress, // pass as-is
    cc: null,
    subject: subject,
    body: body,
    csvContent: errorCsv,
    fileName: $"Error_{DateTime.Now:yyyyMMddHHmmss}.csv"
);





            }
            catch (Exception emailEx)
            {
                _logger.LogError(emailEx, "Failed to send error email notification");
            }
        }
    }
}


