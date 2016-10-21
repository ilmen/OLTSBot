using System;
using OurLittleTastyServerBot.Classes.Db.Models;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string message, UpdateRecord record)
        {
            Message = message;
            Record = record;
        }

        public string Message { get; set; }

        public UpdateRecord Record { get; private set; }
    }
}