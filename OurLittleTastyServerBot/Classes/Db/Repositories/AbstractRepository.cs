using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OurLittleTastyServerBot.Classes.Db.Models;

namespace OurLittleTastyServerBot.Classes.Db.Repositories
{
    public abstract class AbstractRepository<TRecord>
        where TRecord : IRecordWithIdentity
    {
        protected ImmutableHashSet<TRecord> Collection = ImmutableHashSet<TRecord>.Empty;

        public IEnumerable<TRecord> GetAll()
        {
            return Collection;
        }

        public TRecord GetOne(Int32 id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        public Result<TRecord> Insert(TRecord value)
        {
            if (Collection.Contains(value))
            {
                return Result.Fail<TRecord>("������� �������� ��� ������������ ������ Update � ���������!");
            }

            var idResult = InsertIntoDb(value);
            if (idResult.IsFailured) return Result.Fail<TRecord>(idResult);

            // TODO ����� ����������� �� �� ������ �� ����������� Id - �������� �������� ������ � �� (����� ��? ���� ������� ��������)

            var stored = GetCopyWithNewIdentity(value, idResult.Value);
            stored.ThrowIfFailure();    // �������� ������ Id � ���������� ������, ��� ������ ���� ��������� ������

            Collection = Collection.Add(stored.Value);

            return Result.Ok(stored.Value);
        }

        public Result Update(TRecord value)
        {
            if (!Collection.Contains(value))
            {
                return Result.Fail("������� ���������� �� ������������ ������ Update � ���������!");
            }

            var result = UpdateIntoDb(value);
            return result;
        }

        public Result Delete(TRecord value)
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

        protected abstract Result<List<TRecord>> SelectFromDb();
        protected abstract Result<Int32> InsertIntoDb(TRecord value);
        protected abstract Result DeleteFromDb(Int32 id);
        protected abstract Result UpdateIntoDb(TRecord value);
        protected abstract Result<TRecord> GetCopyWithNewIdentity(TRecord source, Int32 id);
    }
}