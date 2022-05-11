using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace Flipdish.Recruiting.WebhookReceiver
{
    public interface IMailMessageBuilder
    {
        MailMessageBuilder Attachments(IDictionary<string, Stream> attachements);
        MailMessageBuilder Body(string body);
        MailMessage Build();
        MailMessageBuilder Cc(IEnumerable<string> cc = null);
        MailMessageBuilder From(string from);
        MailMessageBuilder Subject(string subject);
        MailMessageBuilder To(IEnumerable<string> to);
    }
}