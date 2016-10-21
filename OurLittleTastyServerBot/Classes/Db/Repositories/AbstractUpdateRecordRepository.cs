using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OurLittleTastyServerBot.Classes.Db.Models;

namespace OurLittleTastyServerBot.Classes.Db.Repositories
{
    public abstract class AbstractUpdateRecordRepository
    {
        protected ImmutableHashSet<UpdateRecord> Collection = ImmutableHashSet<UpdateRecord>.Empty;

        public IEnumerable<UpdateRecord> GetAll()
        {
            return Collection;
        }

        public UpdateRecord GetOne(Int32 id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        [SuppressMessage("ReSharper", "RedundantArgumentName")]
        public Result<UpdateRecord> Insert(UpdateRecord value)
        {
            if (Collection.Contains(value))
            {
                return Result.Fail<UpdateRecord>("������� �������� ��� ������������ ������ Update � ���������!");
            }

            var idResult = InsertIntoDb(value);
            if (idResult.IsFailured) return Result.Fail<UpdateRecord>(idResult);

            // TODO ����� ����������� �� �� ������ �� ����������� Id - �������� �������� ������ � �� (����� ��? ���� ������� ��������)

            var stored = UpdateRecord.Factory.Update(value, id: idResult.Value);
            stored.ThrowIfFailure();    // �������� ������ Id � ���������� ������, ��� ������ ���� ��������� ������

            Collection = Collection.Add(stored.Value);

            return Result.Ok(stored.Value);
        }

        public Result Update(UpdateRecord value)
        {
            if (!Collection.Contains(value))
            {
                return Result.Fail("������� ���������� �� ������������ ������ Update � ���������!");
            }

            var result = UpdateIntoDb(value);
            return result;
        }

        public Result Delete(UpdateRecord value)
        {
            if (!Collection.Contains(value))
            {
                return Result.Fail("������� ������� �� ������������ ������ Update �� ���������!");
            }

            var result = DeleteFromDb(value.Id);
            if (result.IsFailured) return result;

            foreach (var item in Collection.Where(x => x.Id == value.Id))
            {
                Collection = Collection.Remove(item);
            }

            return Result.Ok();
        }

        protected abstract Result<List<UpdateRecord>> SelectFromDb();
        protected abstract Result<Int32> InsertIntoDb(UpdateRecord value);
        protected abstract Result DeleteFromDb(Int32 id);
        protected abstract Result UpdateIntoDb(UpdateRecord value);
    }
}