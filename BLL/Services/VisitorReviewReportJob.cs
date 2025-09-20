

using Azure.Communication.Email;
using BLL.IService;
using DAL.IRepositories;
using iRely.Common;
using Model_New.Models;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class VisitorReviewReportJob : IVisitorReviewReportJob
    {
        private readonly IVisitorReviewReportRepository _reviewRepo;
        private readonly IEmailService _emailService;

        public VisitorReviewReportJob(
            IVisitorReviewReportRepository reviewRepo,
            IEmailService emailService)
        {
            _reviewRepo = reviewRepo;
            _emailService = emailService;
        }

        public async Task SendTodayLoginDurationReportAsync()
        {
            try
            {
                var reports = await _reviewRepo.GetTodayLoginDurationReportAsync();

                if (reports != null && reports.Any())
                {
                    // Build email content
                    var subject = $"MR Login Duration Report - {DateTime.UtcNow:dd-MMM-yyyy}";
                    var bodyHtml = BuildLoginDurationEmailBody(reports);

                    // Send CSV attachment with HTML body
                    await _emailService.SendVisitorLoginDurationReportAsync(reports, bodyHtml, subject);
                }
            }
            catch (Exception ex)
            {
                // Log error
                var errorLog = new ApplicationErrorLog
                {
                    ErrorLogId = Guid.NewGuid(),
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    Endpoint = nameof(VisitorReviewReportJob),
                    LoggedAt = DateTime.UtcNow,
                    Status = "Failed",
                    Username = "SystemJob"
                };

                // Send error notification
                await _emailService.SendErrorNotificationAsync(
                    errorLog,
                    $"Error Sending Visitor Login Duration Report - {DateTime.UtcNow:dd-MMM-yyyy}",
                    $"An error occurred while sending the visitor login duration report: {ex.Message}",
                    isHighPriority: true
                );
            }
        }

        // Helper to build email body for login duration report
        private string BuildLoginDurationEmailBody(IEnumerable<VisitorLoginDurationDto> reports)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<p>Dear Manager </p>");
            sb.AppendLine("<p>For your reference, please find the attached report and the details below:</p>");

            sb.AppendLine("<table style='border-collapse: collapse; width: 100%;' border='1' cellpadding='5'>");
            sb.AppendLine("<tr style='background-color: #f2f2f2;'>" +
                "<th>User</th>" +
                "<th>Capture Time</th>" +
                "<th>Logout Time</th>" +
                "<th>Last Activity</th>" +
                "<th>Duration</th>" +
                "<th>Blob URL</th>" +  // Added header for BlobUrl
                "</tr>");

            foreach (var r in reports)
            {
                sb.AppendLine(
                    $"<tr>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.Username)}</td>" +
                    $"<td>{r.CaptureDate:dd-MM-yyyy HH:mm}</td>" +
                    $"<td>{(r.LogoutDate.HasValue ? r.LogoutDate.Value.ToString("dd-MM-yyyy HH:mm") : "-")}</td>" +
                    $"<td>{r.LastActivityComputed:dd-MM-yyyy HH:mm}</td>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.Duration_HMS)}</td>" +
                    $"<td>{(string.IsNullOrEmpty(r.BlobUrl) ? "-" : $"<a href='{r.BlobUrl}' target='_blank'>Link</a>")}</td>" +
                    $"</tr>"
                );
            }


            sb.AppendLine("</table>");
            sb.AppendLine("<br/><p>Thank you,<br/>Azure Support Team</p>");
            return sb.ToString();
        }


        public async Task SendTodayVisitorReviewReportAsync()
        {
            try
            {
                var reports = await _reviewRepo.GetTodayReviewReportAsync();

                if (reports != null && reports.Any())
                {
                    // Build email content
                    var subject = $"Visitor Review Report - {DateTime.UtcNow:dd-MMM-yyyy}";
                    var bodyHtml = BuildEmailBody(reports);

                    // Send CSV attachment with HTML body
                    await _emailService.SendVisitorReviewReportAsync(reports, bodyHtml, subject);
                }
            }
            catch (Exception ex)
            {
                // Log error
                var errorLog = new ApplicationErrorLog
                {
                    ErrorLogId = Guid.NewGuid(),
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    Endpoint = nameof(VisitorReviewReportJob),
                    LoggedAt = DateTime.UtcNow,
                    Status = "Failed",
                  
                    Username = "SystemJob"
                };

                // Send error notification
                await _emailService.SendErrorNotificationAsync(
                    errorLog,
                    $"Error Sending Visitor Review Report - {DateTime.UtcNow:dd-MMM-yyyy}",
                    $"An error occurred while sending the visitor review report: {ex.Message}",
                    isHighPriority: true
                );
            }
        }



        //private string BuildEmailBody(IEnumerable<VisitorReportDto> reports)
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine("<p>Dear Account Manager / Area Manager,</p>");
        //    sb.AppendLine("<p>For your reference, please find the attached report and the details below:</p>");

        //    sb.AppendLine("<table style='border-collapse: collapse; width: 100%;' border='1' cellpadding='5'>");
        //    sb.AppendLine("<tr style='background-color: #f2f2f2;'><th>MR</th><th>SR</th><th>Outlet</th><th>Start</th><th>End</th></tr>");

        //    foreach (var r in reports)
        //    {
        //        sb.AppendLine(
        //            $"<tr>" +
        //            $"<td>{System.Net.WebUtility.HtmlEncode(r.)}</td>" +
        //            $"<td>{System.Net.WebUtility.HtmlEncode(r.SrName)}</td>" +
        //            $"<td>{System.Net.WebUtility.HtmlEncode(r.OutletName)}</td>" +
        //            $"<td>{r.ReviewStartTime:dd-MM-yyyy HH:mm}</td>" +
        //            $"<td>{r.ReviewEndTime:dd-MM-yyyy HH:mm}</td>" +
        //            $"</tr>"
        //        );
        //    }

        //    sb.AppendLine("</table>");
        //    sb.AppendLine("<br/><p>Thank you,<br/>Azure Support Team</p>");
        //    return sb.ToString();
        //}

        private string BuildEmailBody(IEnumerable<VisitorReportDto> reports)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<p>Dear Account Manager / Area Manager,</p>");
            sb.AppendLine("<p>For your reference, please find the attached report and the details below:</p>");

            sb.AppendLine("<table style='border-collapse: collapse; width: 100%;' border='1' cellpadding='5'>");
            sb.AppendLine("<tr style='background-color: #f2f2f2;'>" +
                          "<th>Rscode</th><th>MR Name</th><th>MR No</th><th>SR Name</th><th>Outlet</th><th>Start</th><th>End</th><th>Status</th><th>Time Spent</th></tr>");

            foreach (var r in reports)
            {
                sb.AppendLine(
                    $"<tr>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.Rscode)}</td>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.MRName)}</td>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.MRNo)}</td>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.SrName)}</td>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.OutletName)}</td>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.ReviewStartTime)}</td>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.ReviewEndTime)}</td>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.Status)}</td>" +
                    $"<td>{System.Net.WebUtility.HtmlEncode(r.TimeSpent)}</td>" +
                    $"</tr>"
                );
            }

            sb.AppendLine("</table>");
            sb.AppendLine("<br/><p>Thank you,<br/>Azure Support Team</p>");
            return sb.ToString();
        }






        public async Task SendTodayLoginReportsAsync()
        {
            try
            {
                var report = await _reviewRepo.GetTodayLoginDurationReportAsync1();

                if (report != null && report.Any())
                {
                    foreach (var user in report)
                    {
                        if (string.IsNullOrWhiteSpace(user.EmpEmail))
                            continue;

                        // Email body
                        var body = $@"
Hello {user.EmpName},

Your login report for today:

First Login: {user.FirstLogin:dd-MM-yyyy HH:mm}
Last Activity: {user.LastActivity:dd-MM-yyyy HH:mm}
Total Duration: {user.Duration_HMS}
";

                        // Generate CSV content
                        var csvBuilder = new StringBuilder();
                        csvBuilder.AppendLine("UserId,EmpName,EmpEmail,Username,FirstLogin,LastActivity,Duration_HMS");
                        csvBuilder.AppendLine($"{user.UserId},{user.EmpName},{user.EmpEmail},{user.Username},{user.FirstLogin:yyyy-MM-dd HH:mm:ss},{user.LastActivity:yyyy-MM-dd HH:mm:ss},{user.Duration_HMS}");

                        var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

                        // Create attachment using Azure EmailAttachment constructor
                        var attachment = new EmailAttachment(
                            name: "TodayLoginReport.csv",                     // file name
                            contentType: "text/csv",                          // MIME type
                            content: BinaryData.FromBytes(csvBytes)           // file content
                        );

                        try
                        {
                            await _emailService.SendEmailAsync(
                                to: new List<string> { user.EmpEmail },
                                subject: "Today’s Login Report",
                                body: body,
                                attachments: new List<EmailAttachment> { attachment }
                            );
                        }
                        catch (Exception exUser)
                        {
                            // Log individual email errors
                            var errorLog = new ApplicationErrorLog
                            {
                                ErrorLogId = Guid.NewGuid(),
                                ErrorMessage = exUser.Message,
                                StackTrace = exUser.StackTrace,
                                Endpoint = nameof(VisitorReviewReportJob),
                                LoggedAt = DateTime.UtcNow,
                                Status = "Failed",
                                Username = user.Username ?? "Unknown"
                            };

                            await _emailService.SendErrorNotificationAsync(
                                errorLog,
                                $"Error Sending Visitor Login Report to {user.EmpEmail} - {DateTime.UtcNow:dd-MMM-yyyy}",
                                $"An error occurred while sending the visitor login report to {user.EmpEmail}: {exUser.Message}",
                                isHighPriority: true
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorLog = new ApplicationErrorLog
                {
                    ErrorLogId = Guid.NewGuid(),
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    Endpoint = nameof(VisitorReviewReportJob),
                    LoggedAt = DateTime.UtcNow,
                    Status = "Failed",
                    Username = "SystemJob"
                };

                await _emailService.SendErrorNotificationAsync(
                    errorLog,
                    $"Error Sending Individual Visitor Login Reports - {DateTime.UtcNow:dd-MMM-yyyy}",
                    $"An error occurred while sending the visitor login reports: {ex.Message}",
                    isHighPriority: true
                );
            }
        }










    }
}


