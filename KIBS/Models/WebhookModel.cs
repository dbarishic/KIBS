using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KIBS.Models
{
    public class WebhookModel
    {
        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("entry")]
        public List<Entry> Entry { get; set; }
    }

    public class Entry
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("time")]
        public long Time { get; set; }
        [JsonProperty("messaging")]
        public List<Messaging> Messaging { get; set; }
    }

    public class Messaging
    {
        [JsonProperty("sender")]
        public Sender Sender { get; set; }
        [JsonProperty("recipient")]
        public Recipient Recipient { get; set; }
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
        [JsonProperty("message")]
        public Message Message { get; set; }
        [JsonProperty("postback")]
        public Postback Postback { get; set; }
    }

    public class Postback
    {
        [JsonProperty("payload")]
        public string Payload { get; set; }
    }

    public class Sender
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Recipient
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public Recipient(string id) => Id = id;
    }

    public class Message
    {
        [JsonProperty("mid")]
        public string Mid { get; set; }
        [JsonProperty("seq")]
        public int Seq { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
