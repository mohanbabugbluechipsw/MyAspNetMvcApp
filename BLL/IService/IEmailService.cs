using Azure.Communication.Email;
using Model_New.Models;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.IService
{
   
   

    public interface IEmailService
    {
        Task SendErrorNotificationAsync(ApplicationErrorLog errorLog, string subject, string body, bool isHighPriority = false);
        Task SendReminderEmailAsync(ApplicationErrorLog error);
        Task SendResolutionNotificationAsync(ApplicationErrorLog errorLog, string subject, string body);





        Task SendVisitorReviewReportAsync(
    IEnumerable<VisitorReportDto> data,
    string htmlBody,
    string subject);


        Task SendVisitorLoginDurationReportAsync(IEnumerable<VisitorLoginDurationDto> reports, string bodyHtml, string subject);

        //Task SendVisitorReviewReportAsync(IEnumerable<VisitorReportDto> data);

        Task SendEmailAsync(
        IEnumerable<string> to,
        string subject,
        string body,
        IEnumerable<string>? cc = null,
        IEnumerable<string>? bcc = null,
        IEnumerable<EmailAttachment>? attachments = null
    );
        //Task SendEmailAsync(string to, string subject, string body);

    }





}
