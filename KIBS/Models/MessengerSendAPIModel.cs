using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KIBS.Models
{
    public class MessageText
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        public MessageText(string text) => Text = text;
    }

    public class MessengerSendAPIModel
    {
        [JsonProperty("messaging_type")]
        public string MessagingType { get; set; }
        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
        [JsonProperty("message")]
        public MessageText MessageText { get; set; }
    }
}
