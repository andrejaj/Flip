using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace Flipdish.Recruiting.WebhookReceiver
{

    internal class MailMessageBuilder : IMailMessageBuilder
    {
        private readonly MailMessage _mailMessage;

        public MailMessageBuilder()
        {
            this._mailMessage = new MailMessage();
            this._mailMessage.IsBodyHtml = true;
        }

        public MailMessage Build() => _mailMessage;

        public MailMessageBuilder Body(string body)
        {
            this._mailMessage.Body = body;
            return this;
        }

        public MailMessageBuilder Attachments(IDictionary<string, Stream> attachements)
        {
            foreach (var nameAndStreamPair in attachements)
            {
                var attachment = new Attachment(nameAndStreamPair.Value, nameAndStreamPair.Key)
                {
                    ContentId = nameAndStreamPair.Key
                };
                _mailMessage.Attachments.Add(attachment);
            }
            return this;
        }

        public MailMessageBuilder Cc(IEnumerable<string> cc = null)
        {
            if (cc != null)
            {
                foreach (var t in cc)
                {
                    _mailMessage.To.Add(t);
                }
            }
            return this;
        }

        public MailMessageBuilder From(string from)
        {
            _mailMessage.From = new MailAddress(from);
            return this;
        }

        public MailMessageBuilder To(IEnumerable<string> to)
        {
            foreach (var t in to)
            {
                _mailMessage.To.Add(t);
            }
            return this;
        }

        public MailMessageBuilder Subject(string subject)
        {
            _mailMessage.Subject = subject;
            return this;
        }
    }
}
