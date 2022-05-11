using System;

namespace Flipdish.Recruiting.WebhookReceiver
{
    public interface IEmailRenderer : IDisposable
    {
        string RenderEmailOrder();
    }
}