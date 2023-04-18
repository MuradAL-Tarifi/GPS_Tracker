using GPS.DataAccess.Repository.SystemSettings;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Integration.EmailIntegrations
{
    public class EmailIntegration : IEmailIntegration
    {
        private readonly AppSettings _appSettings;

        public EmailIntegration(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<bool> SendEmailAsync(string subject, string body, string[] toEmails, SystemSetting systemSetting)
        {
            try
            {
                GPSHelper.LogHistory($"SMTP_Address: {systemSetting.SMTP_Address}, " +
                    $"SMTP_PORT: {systemSetting.SMTP_PORT},SMTP_HOST: {systemSetting.SMTP_HOST},SMTP_Password: {systemSetting.SMTP_Password},SMTP_IsSslEnabled: {systemSetting.SMTP_IsSslEnabled}," +
                    $"EnableSMTP: {systemSetting.EnableSMTP}");
                if (systemSetting != null &&
                    systemSetting.SMTP_PORT > 0 &&
                    !string.IsNullOrEmpty(systemSetting.SMTP_Password) &&
                    !string.IsNullOrEmpty(systemSetting.SMTP_HOST)&&
                    !string.IsNullOrEmpty(systemSetting.SMTP_Address))
                {
                    if (!systemSetting.EnableSMTP)
                    {
                        return false;
                    }
                    MailAddress from = new MailAddress(systemSetting.SMTP_Address, systemSetting.SMTP_DisplayName);

                    MailMessage mail = new MailMessage
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                        From = from
                    };

                    foreach (var email in toEmails)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            mail.To.Add(new MailAddress(email));
                        }
                    }

                    SmtpClient client = new SmtpClient
                    {
                        Host = systemSetting.SMTP_HOST,
                        Port = systemSetting.SMTP_PORT,
                        EnableSsl = systemSetting.SMTP_IsSslEnabled,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new System.Net.NetworkCredential(systemSetting.SMTP_Address, systemSetting.SMTP_Password),
                    };

                    await client.SendMailAsync(mail);
                    return true;
                }
               
            }
            catch (Exception ex)
            {
                GPSHelper.LogHistory(ex.Message);
            }
            return false;
        }

        public async Task<bool> SendEmailWithAttachmentsAsync(string subject, string body, string[] toEmails, SystemSetting systemSetting, List<EmailAttachmentModel> emailAttachments)
        {
            try
            {
                GPSHelper.LogHistory($"SMTP_Address: {systemSetting.SMTP_Address}, " +
                    $"SMTP_PORT: {systemSetting.SMTP_PORT},SMTP_HOST: {systemSetting.SMTP_HOST},SMTP_Password: {systemSetting.SMTP_Password},SMTP_IsSslEnabled: {systemSetting.SMTP_IsSslEnabled}," +
                    $"EnableSMTP: {systemSetting.EnableSMTP}");
               
                if (systemSetting != null &&
                    systemSetting.SMTP_PORT > 0 &&
                    !string.IsNullOrEmpty(systemSetting.SMTP_Password) &&
                    !string.IsNullOrEmpty(systemSetting.SMTP_HOST) &&
                    !string.IsNullOrEmpty(systemSetting.SMTP_Address))
                {
                    if (!systemSetting.EnableSMTP)
                    {
                        return false;
                    }
                    MailAddress from = new MailAddress(systemSetting.SMTP_Address, systemSetting.SMTP_DisplayName);

                    MailMessage mail = new MailMessage
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                        From = from,
                    };

                    foreach (var emailAttachment in emailAttachments)
                    {
                        mail.Attachments.Add(new Attachment(new MemoryStream(emailAttachment.Content), emailAttachment.Name));
                    }

                    foreach (var email in toEmails)
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            mail.To.Add(new MailAddress(email));
                        }
                    }

                    SmtpClient client = new SmtpClient
                    {
                        Host = systemSetting.SMTP_HOST,
                        Port = systemSetting.SMTP_PORT,
                        EnableSsl = systemSetting.SMTP_IsSslEnabled,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new System.Net.NetworkCredential(systemSetting.SMTP_Address, systemSetting.SMTP_Password),
                    };

                    await client.SendMailAsync(mail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                GPSHelper.LogHistory(ex.Message);
            }
            return false;
        }
    }
}
