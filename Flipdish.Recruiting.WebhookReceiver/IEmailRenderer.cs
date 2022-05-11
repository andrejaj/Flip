using System;

namespace Flipdish.Recruiting.WebhookReceiver
{
    internal interface IEmailRenderer : IDisposable
    {
        string RenderEmailOrder();
    }
}