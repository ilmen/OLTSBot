using System;
using Newtonsoft.Json;

namespace OurLittleTastyServerBot.Classes.Telegram.Models
{
    public class Message
    {
        public Chat chat { get; set; }
        public Int32 message_id { get; set; }
        public User from { get; set; }

        [JsonProperty("date")]
        public Int64 UtcDateTimeInUnitFormat { get; set; }
        public string text { get; set; }
    }
}