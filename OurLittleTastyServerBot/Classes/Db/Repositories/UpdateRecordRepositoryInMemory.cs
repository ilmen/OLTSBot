using OurLittleTastyServerBot.Classes.Db.Models;

namespace OurLittleTastyServerBot.Classes.Db.Repositories
{
    public class UpdateRecordRepositoryInMemory : AbstractRepository<UpdateRecord>
    {
        protected override Result<UpdateRecord> GetCopyWithNewIdentity(UpdateRecord source, int id)
        {
            // ReSharper disable once RedundantArgumentName
            return UpdateRecord.Factory.Update(source, id: id);
        }
    }
}