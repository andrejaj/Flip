using System;
using System.Collections.Generic;
using System.IO;

namespace Flipdish.Recruiting.WebhookReceiver
{
    public interface IEmailRenderer : IDisposable
    {
        public IDictionary<string, Stream> ImagesWithNames { get; }
        string RenderEmailOrder();
    }
}