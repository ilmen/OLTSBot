using System;

namespace OurLittleTastyServerBot.Classes.Telegram.Models
{
    public class User
    {
        public Int32 id { get; set; }
        public String first_name { get; set; }
        public String last_name { get; set; }
        public String username { get; set; }
    }
}