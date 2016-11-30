using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OurLittleTastyServerBot.Classes.Db.Models;
using System.Data.SQLite;

namespace OurLittleTastyServerBot.Classes.Db.Repositories
{
    // ReSharper disable once InconsistentNaming
    public sealed class UpdateRecordRepositoryInSQLite : AbstractRepository<UpdateRecord>
    {
        private readonly SqliteDatabase _db;

        public UpdateRecordRepositoryInSQLite(SqliteDatabase db)
        {
            _db = db;

            var result = SelectFromDb();
            var records = !result.IsFailured ? result.Value : new List<UpdateRecord>();
            foreach (var item in records)
            {
                Collection = Collection.Add(item);
            }
        }

        protected override Result<List<UpdateRecord>> SelectFromDb()
        {
            // ReSharper disable once InconsistentNaming
            const string QUERY = "select * from updates";

            var result = _db.Select(QUERY);
            if (result.IsFailured) return result.FailCastTo<List<UpdateRecord>>();

            var parseResults = result.Value.Rows.Cast<DataRow>()
                .Select(Parse)
                .ToArray();

            var failure = parseResults.FirstOrDefault(x => x.IsFailured);
            if (failure != null && System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debug.Assert(false, "Ошибка парсинга UpdateRecord!");

            var updates = parseResults
                .Where(x => !x.IsFailured)
                .Select(x => x.Value)
                .ToList();

            return Result.Ok(updates);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected override Result<Int32> InsertIntoDb(UpdateRecord value)
        {
            // TODO FIXME: Сделать защиту от sql вставок, проверить работает ли как защита SQLiteParameter

            const string INSERT_QUERY = @"insert into updates values (?,?,?,?,?,?,?,?,?)";
            var insertResult = _db.ExecuteNonQuery(INSERT_QUERY,
                new SQLiteParameter("@id"),     // autoincrement
                new SQLiteParameter("@updateOuterId", value.UpdateOuterId),
                new SQLiteParameter("@messageOuterId", value.MessageOuterId),
                new SQLiteParameter("@sendTime", UnixDateTime.ToUnixFormat(value.SendTime)),
                new SQLiteParameter("@insertTime", UnixDateTime.ToUnixFormat(value.InsertTime)),
                new SQLiteParameter("@text", value.Text),
                new SQLiteParameter("@chatOuterId", value.ChatOuterId),
                new SQLiteParameter("@userOuterId", value.UserOuterId),
                new SQLiteParameter("@userName", value.UserName));
            if (insertResult.IsFailured) return insertResult.FailCastTo<Int32>();

            var idQuery =
                String.Format(
                    "select id from updates where updateOuterId={0} and messageOuterId={1} and chatOuterId={2} and userOuterId={3}",
                    value.UpdateOuterId,
                    value.MessageOuterId,
                    value.ChatOuterId,
                    value.UserOuterId);
            var idResult = _db.SelectValue(idQuery, Convert.ToInt32);
            if (idResult.IsFailured) return idResult;
            var id = idResult.Value;

            return Result.Ok(id);
        }

        protected override Result DeleteFromDb(Int32 id)
        {
            try
            {
                var query = "delete from updates where id=" + id;
                return _db.ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                return Result.Fail("Ошибка удаления из таблицы updates. Id=" + id, e);
            }
        }

        protected override Result UpdateIntoDb(UpdateRecord value)
        {
            return Result.Fail("Обновление записей пока не поддерживается (т.к. не нужно для сообщений пока)", new NotImplementedException());
        }

        protected override Result<UpdateRecord> GetCopyWithNewIdentity(UpdateRecord source, Int32 id)
        {
            // ReSharper disable once RedundantArgumentName
            return UpdateRecord.Factory.Update(source, id: id);
        }

        private static Result<UpdateRecord> Parse(DataRow row)
        {
            try
            {
                var id = Convert.ToInt32(row["id"]);
                var originalId = Convert.ToInt32(row["updateOuterId"]);
                var messageId = Convert.ToInt32(row["messageOuterId"]);
                var time = UnixDateTime.FromUnixFormat((Int64)row["sendTime"]);
                var insertTime = UnixDateTime.FromUnixFormat((Int64)row["insertTime"]);
                var text = row["text"].ToString().Trim();
                var chatId = Convert.ToInt32(row["chatOuterId"]);
                var userId = Convert.ToInt32(row["userOuterId"]);
                var login = row["userName"].ToString().Trim();

                var update = UpdateRecord.Factory.Create(id, originalId, messageId, time, insertTime, text, chatId, userId, login);
                return update;
            }
            catch (Exception ex)
            {
                return Result.Fail<UpdateRecord>("Ошибка парсинга UpdateRecord!", ex);
            }
        }
    }
}