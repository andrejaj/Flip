using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Flipdish.Recruiting.WebhookReceiver
{
    internal class EmailService
    {
        public static async Task Send(MailMessage mailMessage)
        {
            using SmtpClient mailer = new SmtpClient
            {
                Host = "",
                Credentials = new System.Net.NetworkCredential("", "")
            };
            await mailer.SendMailAsync(mailMessage);
        }
    }
}
