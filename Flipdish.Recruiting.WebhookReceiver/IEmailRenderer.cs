using System;
using System.Collections.Generic;
using System.IO;

namespace Flipdish.Recruiting.WebhookReceiver
{
    internal interface IEmailRenderer : IDisposable
    {
        public IDictionary<string, Stream> ImagesWithNames { get; }
        string RenderEmailOrder();
    }
}