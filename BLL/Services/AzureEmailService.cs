using Azure;
using Azure.Communication.Email;
using BLL.IService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AzureEmailService : IStoreReviewEmailService
    {
        private readonly EmailClient _client;
        private readonly string _sender;

        public AzureEmailService(string connectionString, string senderAddress)
        {
            _client = new EmailClient(connectionString);
            _sender = senderAddress;
        }

        //public async Task SendEmailAsync(string to, string cc, string subject, string body, string csvContent, string fileName)
        //{
        //    var emailMessage = new EmailMessage(
        //        senderAddress: _sender,
        //        content: new EmailContent(subject)
        //        {
        //            PlainText = body
        //        },
        //        recipients: new EmailRecipients(
        //            to: new List<EmailAddress> { new EmailAddress(to) },
        //            cc: string.IsNullOrEmpty(cc) ? null : new List<EmailAddress> { new EmailAddress(cc) }
        //        )
        //    );

        //    // ✅ Correct CSV attachment
        //    var attachment = new EmailAttachment(
        //        name: fileName,
        //        contentType: "text/csv",
        //        content: BinaryData.FromBytes(Encoding.UTF8.GetBytes(csvContent))
        //    );

        //    emailMessage.Attachments.Add(attachment);

        //    await _client.SendAsync(WaitUntil.Completed, emailMessage);
        //}





        //public async Task SendEmailAsync(string to, string cc, string subject, string body, string csvContent, string fileName)
        //{
        //    var emailMessage = new EmailMessage(
        //        senderAddress: _sender,
        //        content: new EmailContent(subject)
        //        {
        //            Html = body
        //        },
        //        recipients: new EmailRecipients(
        //            to: new List<EmailAddress> { new EmailAddress(to) },
        //            cc: string.IsNullOrEmpty(cc)
        //                    ? null
        //                    : cc.Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                        .Select(e => new EmailAddress(e.Trim()))
        //                        .ToList()
        //        )
        //    );

        //    var attachment = new EmailAttachment(
        //        name: fileName,
        //        contentType: "text/csv",
        //        content: BinaryData.FromBytes(Encoding.UTF8.GetBytes(csvContent))
        //    );

        //    emailMessage.Attachments.Add(attachment);

        //    await _client.SendAsync(WaitUntil.Completed, emailMessage);
        //}



        public async Task SendEmailAsync(string to, string cc, string subject, string body, string csvContent, string fileName)
        {
            // Split and trim all TO addresses
            var toRecipients = to.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(e => new EmailAddress(e.Trim()))
                                 .ToList();

            // Split and trim all CC addresses (if any)
            var ccRecipients = string.IsNullOrEmpty(cc)
                ? new List<EmailAddress>()
                : cc.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => new EmailAddress(e.Trim()))
                    .ToList();

            var emailMessage = new EmailMessage(
                senderAddress: _sender,
                content: new EmailContent(subject)
                {
                    Html = body
                },
                recipients: new EmailRecipients(to: toRecipients, cc: ccRecipients)
            );

            if (!string.IsNullOrEmpty(csvContent) && !string.IsNullOrEmpty(fileName))
            {
                var attachment = new EmailAttachment(
                    name: fileName,
                    contentType: "text/csv",
                    content: BinaryData.FromBytes(Encoding.UTF8.GetBytes(csvContent))
                );

                emailMessage.Attachments.Add(attachment);
            }

            await _client.SendAsync(WaitUntil.Completed, emailMessage);
        }







    }
}
