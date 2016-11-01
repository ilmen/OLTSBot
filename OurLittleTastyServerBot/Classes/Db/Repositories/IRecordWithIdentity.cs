using System;

namespace OurLittleTastyServerBot.Classes.Db.Repositories
{
    public interface IRecordWithIdentity
    {
        Int32 Id { get; }
    }
}