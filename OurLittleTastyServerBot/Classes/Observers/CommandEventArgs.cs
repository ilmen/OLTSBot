using System;
using OurLittleTastyServerBot.Classes.Db.Models;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public sealed class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(EnCommand command, UpdateRecord record)
        {
            Command = command;
            Record = record;
        }

        public EnCommand Command { get; private set; }

        public UpdateRecord Record { get; private set; }
    }
}