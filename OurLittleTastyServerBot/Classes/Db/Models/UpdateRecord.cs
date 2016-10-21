using System;
using System.Diagnostics.CodeAnalysis;

namespace OurLittleTastyServerBot.Classes.Db.Models
{
    public class UpdateRecord
    {
        public static class Factory
        {
            private static Result<UpdateRecord> CreateIfValid(bool isNew, Int32 id, Int32 updateOuterId, Int32 messageOuterId, DateTime time, string text, Int32 chatOuterId, Int32 userOuterId, string userName)
            {
                var record = new UpdateRecord
                {
                    Id = id,
                    UpdateOuterId = updateOuterId,
                    MessageOuterId = messageOuterId,
                    Time = time,
                    Text = text,
                    ChatOuterId = chatOuterId,
                    UserOuterId = userOuterId,
                    UserName = userName
                };
                var validateResult = Validate(record, isNew);
                return !validateResult.IsFailured ? Result.Ok(record) : Result.Fail<UpdateRecord>(validateResult);
            }

            public static Result<UpdateRecord> Create(Int32 id, Int32 updateOuterId, Int32 messageOuterId, DateTime time, string text, Int32 chatOuterId, Int32 userOuterId, string userName)
            {
                return CreateIfValid(false, id, updateOuterId, messageOuterId, time, text, chatOuterId, userOuterId, userName);
            }

            public static Result<UpdateRecord> CreateEmpty(Int32 updateOuterId, Int32 messageOuterId, DateTime time, string text, Int32 chatOuterId, Int32 userOuterId, string userName)
            {
                return CreateIfValid(true, 0, updateOuterId, messageOuterId, time, text, chatOuterId, userOuterId, userName);
            }

            public static Result<UpdateRecord> Update(UpdateRecord value,
                Int32? id = null,
                Int32? updateOuterId = null,
                Int32? messageOuterId = null,
                DateTime? time = null,
                string text = null,
                Int32? chatOuterId = null,
                Int32? userOuterId = null,
                string userName = null)
            {
                var result = CreateIfValid(false, id ?? value.Id, updateOuterId ?? value.UpdateOuterId,
                    messageOuterId ?? value.MessageOuterId, time ?? value.Time, text ?? value.Text,
                    chatOuterId ?? value.ChatOuterId, userOuterId ?? value.UserOuterId, userName ?? value.UserName);
                return result;
            }

            [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
            [SuppressMessage("ReSharper", "InconsistentNaming")]
            private static Result Validate(UpdateRecord value, bool IsNew)
            {
                const Int32 MIN_FOREIGN_KEY = 1;
                const Int32 MIN_PRIMARY_KEY = 1;

                if (IsNew)
                {
                    if (value.Id < 0) return Result.Fail("Incorrect Id!");
                }
                else
                {
                    if (value.Id < MIN_PRIMARY_KEY) return Result.Fail("Incorrect Id (as primary key)!");
                }

                if (value.UpdateOuterId < MIN_FOREIGN_KEY) return Result.Fail("Incorrect UpdateOuterId!");
                if (value.MessageOuterId < MIN_FOREIGN_KEY) return Result.Fail("Incorrect MessageOuterId!");
                // now time is not checked
                if (String.IsNullOrEmpty(value.Text)) return Result.Fail("Message text missed!");
                if (value.ChatOuterId < MIN_FOREIGN_KEY) return Result.Fail("Incorrect ChatOuterId!");
                if (value.UserOuterId < MIN_FOREIGN_KEY) return Result.Fail("Incorrect UserOuterId!");
                if (String.IsNullOrEmpty(value.UserName)) return Result.Fail("Interlocutor username missed!");

                return Result.Ok();
            }
        }

        private UpdateRecord()
        {
            // hide constructor for use validation on creating
        }

        public Int32 Id
        { get; private set; }

        public Int32 UpdateOuterId
        { get; private set; }

        public Int32 MessageOuterId
        { get; private set; }

        public DateTime Time
        { get; private set; }

        public string Text
        { get; private set; }

        public Int32 ChatOuterId
        { get; private set; }

        public Int32 UserOuterId
        { get; private set; }

        public string UserName
        { get; private set; }
    }
}