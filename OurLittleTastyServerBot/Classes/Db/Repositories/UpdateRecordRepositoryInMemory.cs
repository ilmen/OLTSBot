using System;
using System.Collections.Generic;
using OurLittleTastyServerBot.Classes.Db.Models;

namespace OurLittleTastyServerBot.Classes.Db.Repositories
{
    public class UpdateRecordRepositoryInMemory : AbstractRepository<UpdateRecord>
    {
        protected override Result<List<UpdateRecord>> SelectFromDb()
        {
            return Result.Ok(new List<UpdateRecord>());
        }

        protected override Result<Int32> InsertIntoDb(UpdateRecord value)
        {
            return Result.Ok(-1);
        }

        protected override Result DeleteFromDb(Int32 id)
        {
            return Result.Ok();
        }

        protected override Result UpdateIntoDb(UpdateRecord value)
        {
            return Result.Ok();
        }

        protected override Result<UpdateRecord> GetCopyWithNewIdentity(UpdateRecord source, int id)
        {
            // ReSharper disable once RedundantArgumentName
            return UpdateRecord.Factory.Update(source, id: id);
        }
    }
}