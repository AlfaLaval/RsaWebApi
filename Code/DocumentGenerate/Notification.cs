using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DocumentGenerate
{
    public class Notification
    {
        public static void SendEmail(string toEmail, string subject, string messageBody, string filePath = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmail))
                    return;

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(AppSettings.HostEmailAddress);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = subject;
                    message.Body = messageBody;
                    message.IsBodyHtml = true;
                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        message.Attachments.Add(new Attachment(filePath));
                    }

                    using (SmtpClient smtp = new SmtpClient(AppSettings.SmtpClientHost, AppSettings.SmtpClientPort))
                    {
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(AppSettings.HostEmailAddress, AppSettings.HostEmailPassord);
                        smtp.EnableSsl = true;
                        smtp.Send(message);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
