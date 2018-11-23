
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net;
using System;

using KIBS.Models;
using System.Net.Http;
using System.Collections.Generic;

namespace KIBS
{
   public static class Function1
    {
        private static readonly string VERIFY_TOKEN = Environment.GetEnvironmentVariable("MESSENGER_VERIFY_TOKEN");
        private static readonly string MESSENGER_API_TOKEN = Environment.GetEnvironmentVariable("MESSENGER_API_TOKEN");
        private static readonly AzureTablesClient _azureTablesClient = new AzureTablesClient();
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly SMSClient _smsClient = new SMSClient();

        private static readonly List<string> numbers = new List<string>
        {
            Environment.GetEnvironmentVariable("DAVOR_TELEFON"),
            Environment.GetEnvironmentVariable("BUGAR_TELEFON"),
            Environment.GetEnvironmentVariable("BOBI_TELEFON")
        };

        [FunctionName("ProcessWebhook")]
        public async static Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "webhook")]HttpRequest req, ILogger log)
        {
            if (req.HttpContext.Request.Method == "GET")
                return await Get(req, log);
            else if (req.HttpContext.Request.Method == "POST")
                return await Post(req, log);
            else
                return new NotFoundResult();
        }

        private async static Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "webhook")]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Parse the query params
            string mode = req.Query["hub.mode"];
            string token = req.Query["hub.verify_token"];
            string challenge = req.Query["hub.challenge"];

            if (mode == "subscribe" && token == VERIFY_TOKEN)
            {
                log.LogInformation("WEBHOOK_VERIFIED");
                return new OkObjectResult($"{challenge}");
            }

            return new BadRequestObjectResult("token invalid or mode is not set to subscribe!");
        }

        private async static Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "webhook")]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            log.LogInformation(requestBody);

            WebhookModel body = JsonConvert.DeserializeObject<WebhookModel>(requestBody);
            var senderId = body.Entry[0].Messaging[0].Sender.Id;

            if (body.Object == "page")
            {
                string message = default;

                foreach (var entry in body.Entry)
                {
                    message = entry.Messaging[0].Message.Text;
                    log.LogInformation($"{message}");
                }

                var currentDateAsString = DateTime.Now.ToString("MMddyyyy");
                var executionLimitReached = await _azureTablesClient.CheckUpdateExecutionLimit(currentDateAsString);

                if (executionLimitReached)
                {
                    var msg = "Pretera sine, 4 puti na den dosta e!";
                    SendMmessage(senderId, msg);
                }
                else
                {
                    var msg = "Pustiv poruki, pali si!";
                    SendMmessage(senderId, msg);
                    var name = await GetNameFromId(senderId);
                    _smsClient.SendSMS(name, "PALI SI", numbers);
                    log.LogInformation("SMS MESSAGE SENT");
                }
                return new OkObjectResult($"{message}");
            }

            return new NotFoundResult();
        }

        private static void SendMmessage(string recipientId, string message)
        {
            var msg = message;
            MessengerSendAPIModel requestBody = new MessengerSendAPIModel
            {
                MessagingType = "RESPONSE",
                MessageText = new MessageText(message),
                Recipient = new Recipient(recipientId)
            };

            PostRaw("https://graph.facebook.com/v2.6/me/messages?access_token=" + $"{MESSENGER_API_TOKEN}", requestBody);
        }

        private async static void PostRaw(string url, MessengerSendAPIModel data)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync<MessengerSendAPIModel>(url, data);
            Console.WriteLine(response.RequestMessage);
            response.EnsureSuccessStatusCode();
        }

        private async static Task<string> GetNameFromId(string psid)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"https://graph.facebook.com/{psid}?fields=first_name&access_token={MESSENGER_API_TOKEN}");
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(responseJson);
            return user.FirstName + user.LastName;
        }
    }
}