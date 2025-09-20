


using Azure;
using Azure.Communication.Email;
using BLL.IService;
using Microsoft.Extensions.Configuration;
using Model_New.Models;
using Model_New.ViewModels;
using System.Text;

namespace BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly string _senderAddress;
        private readonly List<string> _toAddresses;
        private readonly List<string> _ccAddresses;


        private readonly List<string> _visitorToAddresses;
        private readonly List<string> _visitorCcAddresses;

        public EmailService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureCommunicationServices:ConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("ACS connection string is missing from configuration.");

            _emailClient = new EmailClient(connectionString);

            _senderAddress = configuration["AzureCommunicationServices:SenderAddress"];
            if (string.IsNullOrWhiteSpace(_senderAddress))
                throw new InvalidOperationException("Sender address is missing from configuration.");

            _toAddresses = configuration["EmailSettings:RecipientAddress"]
                ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();

            _ccAddresses = configuration["EmailSettings:CcAddress"]
                ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();



            _visitorToAddresses = configuration["EmailSettings:VisitorReportRecipientAddress"]
         ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>();

            _visitorCcAddresses = configuration["EmailSettings:VisitorReportCcAddress"]
                ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>();


        }

        public async Task SendErrorNotificationAsync(ApplicationErrorLog errorLog, string subject, string body, bool isHighPriority = false)
        {
            var csvContent = $"ErrorId,Message,Endpoint,OccurredAt\n" +
                             $"\"{errorLog.ErrorLogId}\",\"{Escape(errorLog.ErrorMessage)}\",\"{Escape(errorLog.Endpoint)}\",\"{errorLog.LoggedAt:yyyy-MM-dd HH:mm:ss}\"";

            var attachment = new EmailAttachment(
                name: "ErrorDetails.csv",
                contentType: "text/csv",
                content: BinaryData.FromBytes(Encoding.UTF8.GetBytes(csvContent))
            );

            var emailMessage = BuildEmailMessage(subject, body, attachment, isHighPriority);
            await TrySendEmailAsync(emailMessage);
        }

        public async Task SendResolutionNotificationAsync(ApplicationErrorLog errorLog, string subject, string body)
        {
            var emailMessage = BuildEmailMessage(subject, body);
            await TrySendEmailAsync(emailMessage);
        }

        public async Task SendReminderEmailAsync(ApplicationErrorLog errorLog)
        {
            var subject = $"Reminder: Unresolved Error - {errorLog.ErrorMessage}";
            var body = new StringBuilder();
            body.AppendLine("<h3>This is a reminder for the following unresolved error:</h3>");
            body.AppendLine($"<p><strong>Error ID:</strong> {errorLog.ErrorLogId}</p>");
            body.AppendLine($"<p><strong>Error Message:</strong> {Escape(errorLog.ErrorMessage)}</p>");
            body.AppendLine($"<p><strong>Endpoint:</strong> {Escape(errorLog.Endpoint)}</p>");
            body.AppendLine($"<p><strong>HTTP Method:</strong> {errorLog.HttpMethod}</p>");
            body.AppendLine($"<p><strong>User Agent:</strong> {errorLog.UserAgent}</p>");
            body.AppendLine($"<p><strong>User IP:</strong> {errorLog.UserIP}</p>");
            body.AppendLine($"<p><strong>Occurred At:</strong> {errorLog.LoggedAt}</p>");
            body.AppendLine($"<p><strong>Status:</strong> {errorLog.Status}</p>");
            body.AppendLine("<p>Please take necessary action to resolve this issue.</p>");

            var emailMessage = BuildEmailMessage(subject, body.ToString());
            await TrySendEmailAsync(emailMessage);
        }

        // --- Private Helpers ---

        private EmailMessage BuildEmailMessage(string subject, string htmlBody, EmailAttachment? attachment = null, bool isHighPriority = false)
        {
            var content = new EmailContent(subject) { Html = htmlBody };

            var toRecipients = _toAddresses.Select(email => new EmailAddress(email)).ToList();
            var ccRecipients = _ccAddresses.Select(email => new EmailAddress(email)).ToList();
            var recipients = new EmailRecipients(toRecipients, ccRecipients);

            var message = new EmailMessage(_senderAddress, recipients, content);

            if (attachment != null)
                message.Attachments.Add(attachment);

            if (isHighPriority)
            {
                message.Headers.Add("X-Priority", "1");
                message.Headers.Add("Importance", "High");
            }

            return message;
        }

        private async Task TrySendEmailAsync(EmailMessage message, int maxRetries = 3)
        {
            int attempt = 0;

            while (attempt < maxRetries)
            {
                try
                {
                    var response = await _emailClient.SendAsync(WaitUntil.Completed, message);
                    Console.WriteLine($"✅ Email sent: {response.Value.Status}");
                    return;
                }
                catch (RequestFailedException ex) when (ex.Status >= 500)
                {
                    attempt++;
                    Console.WriteLine($"⚠️ Temporary failure (attempt {attempt}): {ex.Message}");
                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Email send failed: {ex.Message}");
                    break;
                }
            }
        }

        private string Escape(string? value) =>
            string.IsNullOrWhiteSpace(value) ? "" : value.Replace("\"", "\"\"");




        public async Task SendVisitorReviewReportAsync(
    IEnumerable<VisitorReportDto> data,
    string htmlBody,
    string subject)
        {
            // Build CSV
            var csv = BuildReviewReportCsv(data);

            var attachment = new EmailAttachment(
                name: $"Visit_Summary_Report_{DateTime.UtcNow:yyyyMMdd}.csv",
                contentType: "text/csv",
                content: BinaryData.FromBytes(Encoding.UTF8.GetBytes(csv))
            );

            var content = new EmailContent(subject)
            {
                Html = htmlBody
            };

            var toRecipients = _visitorToAddresses.Select(e => new EmailAddress(e)).ToList();
            var ccRecipients = _visitorCcAddresses.Select(e => new EmailAddress(e)).ToList();
            var recipients = new EmailRecipients(toRecipients, ccRecipients);

            var message = new EmailMessage(_senderAddress, recipients, content);
            message.Attachments.Add(attachment);

            await TrySendEmailAsync(message);
        }

        private string BuildReviewReportCsv(IEnumerable<VisitorReportDto> data)
        {
            var sb = new StringBuilder();

            // Header
            sb.AppendLine("Rscode,RS_Name,MRNo,MRName,SrCode,SrName,OutletCode,RouteName,ChildParty,OutletName,ServicingPLG,ReviewStartTime,ReviewEndTime,Status,TimeSpent,TargetDate");

            foreach (var r in data)
            {
                sb.AppendLine(
                    $"\"{r.Rscode}\",\"{r.RS_Name}\",\"{r.MRNo}\",\"{r.MRName}\",\"{r.SrCode}\",\"{r.SrName}\",\"{r.OutletCode}\",\"{r.RouteName}\",\"{r.ChildParty}\",\"{r.OutletName}\",\"{r.ServicingPLG}\",\"{r.ReviewStartTime}\",\"{r.ReviewEndTime}\",\"{r.Status}\",\"{r.TimeSpent}\",\"{r.TargetDate:yyyy-MM-dd}\""
                );
            }

            return sb.ToString();
        }


        public async Task SendVisitorLoginDurationReportAsync(
     IEnumerable<VisitorLoginDurationDto> reports,
     string bodyHtml,
     string subject)
        {
            if (reports == null || !reports.Any())
                return;

            // Build CSV
            var csv = BuildLoginDurationCsv(reports);

            var attachment = new EmailAttachment(
                name: $"Visitor_Login_Duration_{DateTime.UtcNow:yyyyMMdd}.csv",
                contentType: "text/csv",
                content: BinaryData.FromBytes(Encoding.UTF8.GetBytes(csv))
            );

            var content = new EmailContent(subject)
            {
                Html = bodyHtml
            };

            // Build recipients
            var toRecipients = _visitorToAddresses.Select(e => new EmailAddress(e)).ToList();
            var ccRecipients = _visitorCcAddresses.Select(e => new EmailAddress(e)).ToList();
            var recipients = new EmailRecipients(toRecipients, ccRecipients);

            // Create message
            var message = new EmailMessage(_senderAddress, recipients, content);
            message.Attachments.Add(attachment);

            await TrySendEmailAsync(message);
        }

        // Helper to build CSV from VisitorLoginDurationDto
        private string BuildLoginDurationCsv(IEnumerable<VisitorLoginDurationDto> reports)
        {
            var sb = new StringBuilder();
            sb.AppendLine("UserId,Username,BlobUrl,CaptureDate,LogoutDate,LastActivity,Duration_HMS");

            foreach (var r in reports)
            {
                sb.AppendLine(
                    $"\"{r.UserId}\"," +
                    $"\"{r.Username}\"," +
                     $"\"{r.BlobUrl}\"," +
                    $"\"{r.CaptureDate:yyyy-MM-dd HH:mm:ss}\"," +
                    $"\"{(r.LogoutDate.HasValue ? r.LogoutDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : "")}\"," +
                    $"\"{r.LastActivityComputed:yyyy-MM-dd HH:mm:ss}\"," +
                    $"\"{r.Duration_HMS}\""
                );
            }

            return sb.ToString();
        }
        public async Task SendEmailAsync(
    IEnumerable<string> to,
    string subject,
    string body,
    IEnumerable<string>? cc = null,
    IEnumerable<string>? bcc = null,
    IEnumerable<EmailAttachment>? attachments = null)
        {
            if (to == null || !to.Any())
                throw new ArgumentException("At least one recipient email is required.", nameof(to));

            var toRecipients = to.Select(e => new EmailAddress(e)).ToList();
            var ccRecipients = cc?.Select(e => new EmailAddress(e)).ToList() ?? new List<EmailAddress>();
            var bccRecipients = bcc?.Select(e => new EmailAddress(e)).ToList() ?? new List<EmailAddress>();

            var recipients = new EmailRecipients(toRecipients, ccRecipients, bccRecipients);

            var content = new EmailContent(subject)
            {
                PlainText = body,
                Html = $"<p>{body.Replace("\n", "<br>")}</p>"
            };

            var message = new EmailMessage(_senderAddress, recipients, content);

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                    message.Attachments.Add(attachment);
            }

            try
            {
                await _emailClient.SendAsync(WaitUntil.Completed, message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to send email to {string.Join(", ", to)}.", ex);
            }
        }




        //public async Task SendEmailAsync(string to, string subject, string body)
        //{
        //    if (string.IsNullOrWhiteSpace(to))
        //        throw new ArgumentException("Recipient email is required.", nameof(to));

        //    var emailMessage = new EmailMessage(
        //        senderAddress: _senderAddress,
        //        content: new EmailContent(subject)
        //        {
        //            PlainText = body,
        //            Html = $"<p>{body.Replace("\n", "<br>")}</p>"
        //        },
        //        recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(to) })
        //    );

        //    try
        //    {
        //        await _emailClient.SendAsync(emailMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Optional: log or rethrow exception
        //        throw new InvalidOperationException($"Failed to send email to {to}.", ex);
        //    }
        //}
    }
}

