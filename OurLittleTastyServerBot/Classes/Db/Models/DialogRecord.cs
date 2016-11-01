using System;
using OurLittleTastyServerBot.Classes.Db.Repositories;
using OurLittleTastyServerBot.Classes.Observers;

namespace OurLittleTastyServerBot.Classes.Db.Models
{
    public class DialogRecord : IRecordWithIdentity
    {
        public DialogRecord(Int32 id, Int32 chatId, EnCommand dialogType, Int32 beginMessageId, Int32? endMessageId = null)
        {
            Id = id;
            ChatId = chatId;
            DialogType = dialogType;
            BeginMessageId = beginMessageId;
            EndMessageId = endMessageId;
        }

        public Int32 Id
        { get; private set; }

        public Int32 ChatId
        { get; private set; }

        public EnCommand DialogType
        { get; private set; }

        public Int32 BeginMessageId
        { get; private set; }
        
        public Int32? EndMessageId
        { get; private set; }

        public bool IsEnded
        {
            get { return EndMessageId != null; }
        }

        public static DialogRecord Copy(DialogRecord source, Int32? id = null, Int32? chatId = null, EnCommand? dialogType = null, Int32? beginMessageId = null, Int32? endMessageId = null)
        {
            return new DialogRecord(id ?? source.Id, chatId ?? source.ChatId, dialogType ?? source.DialogType, beginMessageId ?? source.BeginMessageId, endMessageId ?? source.EndMessageId);
        }
    }
}