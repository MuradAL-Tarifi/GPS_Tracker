using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.EmailIntegrations
{
    public interface IEmailIntegration
    {
        Task<bool> SendEmailAsync(string subject, string body, string[] toEmails, SystemSetting systemSetting);

        Task<bool> SendEmailWithAttachmentsAsync(string subject, string body, string[] toEmails, SystemSetting systemSetting, List<EmailAttachmentModel> emailAttachments);


    }
}
