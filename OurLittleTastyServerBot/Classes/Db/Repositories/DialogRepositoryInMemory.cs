using System;
using OurLittleTastyServerBot.Classes.Db.Models;

namespace OurLittleTastyServerBot.Classes.Db.Repositories
{
    public class DialogRepositoryInMemory : AbstractRepository<DialogRecord>
    {
        protected override Result<DialogRecord> GetCopyWithNewIdentity(DialogRecord source, Int32 id)
        {
            // ReSharper disable once RedundantArgumentName
            return Result.Ok(DialogRecord.Copy(source, id: id));
        }
    }
}