using System;

namespace OurLittleTastyServerBot.Classes.Telegram.Models
{
    public class Update
    {
        public Int32 update_id { get; set; }
        public Message message { get; set; }
    }
}