using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace KIBS
{
    public class SMSClient
    {
        public void SendSMS(string sender, string messageText, List<string> numbers)
        { 

            string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            string authToken = Environment.GetEnvironmentVariable("TWILIO_API_TOKEN");

            TwilioClient.Init(accountSid, authToken);

            foreach (var number in numbers)
            {
                var message = MessageResource.Create(
                    body: $"{sender}: {messageText}",
                    from: new Twilio.Types.PhoneNumber(Environment.GetEnvironmentVariable("TWILIO_NUMBER")),
                    to: new Twilio.Types.PhoneNumber(number)
                );
            }
        }
    }
}
