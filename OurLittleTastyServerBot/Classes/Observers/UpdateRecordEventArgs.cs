using System;
using OurLittleTastyServerBot.Classes.Db.Models;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public sealed class UpdateRecordEventArgs : EventArgs
    {
        public UpdateRecord Record { get; private set; }

        public UpdateRecordEventArgs(UpdateRecord record)
        {
            Record = record;
        }
    }
}