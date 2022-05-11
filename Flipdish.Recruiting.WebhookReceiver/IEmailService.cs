using System.Net.Mail;
using System.Threading.Tasks;

namespace Flipdish.Recruiting.WebhookReceiver
{
    internal interface IEmailService
    {
        Task Send(MailMessage mailMessage);
    }
}