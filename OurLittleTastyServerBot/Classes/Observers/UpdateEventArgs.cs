using System;
using OurLittleTastyServerBot.Classes.Telegram.Models;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public class UpdateEventArgs : EventArgs
    {
        public UpdateEventArgs(Update data)
        {
            Data = data;
        }

        public Update Data { get; private set; }
    }
}