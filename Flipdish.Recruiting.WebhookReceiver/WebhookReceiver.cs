using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Flipdish.Recruiting.WebhookReceiver.Models;
using System.Collections.Generic;

namespace Flipdish.Recruiting.WebhookReceiver
{
    public static class WebhookReceiver
    {
        public static IMailMessageBuilder MailMessageBuilder
        {
            get { return new MailMessageBuilder(); }
        }

        public static IEmailRenderer GetEmailRenderer(Order order, string appNameId, string barcodeMetadataKey, string appDirectory, ILogger log, Currency currency)
        {
            return new EmailRenderer(order, appNameId, barcodeMetadataKey, appDirectory, log, currency);
        }

        [FunctionName("WebhookReceiver")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            int? orderId = null;
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string test = req.Query["test"];
                string content = string.Empty;
                if(req.Method == "GET" && !string.IsNullOrEmpty(test))
                {

                    var templateFilePath = Path.Combine(context.FunctionAppDirectory, "TestWebhooks", test);
                    content = new StreamReader(templateFilePath).ReadToEnd();
                }
                else if (req.Method == "POST")
                {
                    content = await new StreamReader(req.Body).ReadToEndAsync();
                }
                else
                {
                    throw new Exception("No body found or test param.");
                }
               
                OrderCreatedEvent orderCreatedEvent = GetOrderCreatedEvent(content);
                orderId = orderCreatedEvent.Order.OrderId;
                if (!IsValidOrderEvent(orderCreatedEvent.Order.Store.Id, req.Query["storeId"].ToArray(), orderId))
                {
                    log.LogInformation($"Skipping order #{orderCreatedEvent.Order.OrderId}");
                    return new ContentResult { Content = $"Skipping order #{orderCreatedEvent.Order.OrderId}", ContentType = "text/html" };
                }

                var currency = GetDefaultCurrency(req.Query["currency"].FirstOrDefault());
                var barcodeMetadataKey = req.Query["metadataKey"].First() ?? "eancode";

                var emailRenderer = GetEmailRenderer(orderCreatedEvent.Order, orderCreatedEvent.AppId, barcodeMetadataKey, context.FunctionAppDirectory, log, currency);
                 var emailOrder = emailRenderer.RenderEmailOrder();

                var mailMessage = MailMessageBuilder
                    .From("")
                    .To(req.Query["to"])
                    .Subject($"New Order #{orderId}")
                    .Body(emailOrder)
                    .Attachments(emailRenderer.ImagesWithNames)
                    .Build();

                try
                {
                    await EmailService.Send(mailMessage);
                }
                catch(Exception ex)
                {
                    log.LogError($"Error occured during sending email for order #{orderId}" + ex);
                }

                log.LogInformation($"Email sent for order #{orderId}.", new { orderCreatedEvent.Order.OrderId });

                return new ContentResult { Content = emailOrder, ContentType = "text/html" };
            }
            catch(Exception ex)
            {
                log.LogError(ex, $"Error occured during processing order #{orderId}");
                throw ex;
            }
        }

        private static OrderCreatedEvent GetOrderCreatedEvent(string content)
        {
            OrderCreatedWebhook orderCreatedWebhook = JsonConvert.DeserializeObject<OrderCreatedWebhook>(content);
            return orderCreatedWebhook.Body;
        }

        private static Currency GetDefaultCurrency(string currencyString)
        {
            Currency currency = Currency.EUR;
            if (!string.IsNullOrEmpty(currencyString) && Enum.TryParse(typeof(Currency), currencyString.ToUpper(), out object currencyObject))
            {
                currency = (Currency)currencyObject;
            }
            return currency;
        }

        private static bool IsValidOrderEvent(int? StoreSummaryId, string[] storeIdsParams, int? OrderId = null)
        {
            if (OrderId is null)
            {
                throw new ArgumentNullException(nameof(OrderId));
            }

            List<int> storeIds = new List<int>();
            string[] storeIdParams = storeIdsParams;
            if (storeIdParams.Length > 0)
            {
                foreach (var storeIdString in storeIdParams)
                {
                    int.TryParse(storeIdString, out int storeId);
                    storeIds.Add(storeId);
                }

                if (!storeIds.Contains(StoreSummaryId.Value))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
