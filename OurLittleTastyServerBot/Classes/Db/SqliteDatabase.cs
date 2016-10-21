using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;

namespace OurLittleTastyServerBot.Classes.Db
{
    public class SqliteDatabase
    {
        private readonly string _filename;

        public SqliteDatabase(string filename)
        {
            _filename = filename;

            if (CheckDbExist()) return;

            var result = InitializeDb();
            if (result.IsFailured && System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.Assert(false, "Ошибка формирования базы данных!");
            }
        }

        private bool CheckDbExist()
        {
            return System.IO.File.Exists(_filename);
        }

        private Result InitializeDb()
        {
            try
            {
                //SQLiteConnection.CreateFile(_filename);   // не нужно, файл автоматически создается при подключении

                ExecuteNonQuery("PRAGMA foreign_keys = ON;");

                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS [updates] (
                    [id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    [updateOuterId] INTEGER,
                    [messageOuterId] INTEGER,
                    [sendTime] INTEGER NOT NULL,
                    [text] char(2000) NOT NULL,
                    [chatOuterId] INTEGER,
                    [userOuterId] INTEGER,
                    [userName] char(100) NOT NULL
                    );");

                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail("Ошибка инициализации базы данных!", e);
            }
        }

        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(string.Format("Data Source={0};", _filename));
        }

        public Result<DataTable> Select(string query)
        {
            return Select(query, new SQLiteParameter[0]);
        }

        public Result<DataTable> Select(string query, params SQLiteParameter[] parameters)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    var dt = new DataTable();
                    using (var da = new SQLiteDataAdapter(query, connection))
                    {
                        da.Fill(dt);
                        return Result.Ok(dt);
                    }
                }
            }
            catch (Exception e)
            {
                return Result.Fail<DataTable>("Ошибка запроса на выборку! Запрос: " + query, e);
            }
        }

        public Result<Int32> ExecuteNonQuery(string query)
        {
            return ExecuteNonQuery(query, new SQLiteParameter[0]);
        }

        public Result<Int32> ExecuteNonQuery(string query, params SQLiteParameter[] parameters)
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddRange(parameters);
                        var rowCount = command.ExecuteNonQuery();
                        return Result.Ok(rowCount);
                    }
                }
            }
            catch (Exception e)
            {
                return Result.Fail<Int32>("Ошибка запроса обновления! Запрос: " + query, e);
            }
        }

        /// <summary> Запрос на выборку значения ячейки из базе данных </summary>
        /// <typeparam name="T"> Тип возвращаемого значения </typeparam>
        /// <param name="query"> Запрос на получение значения из ячейки таблицы в БД </param>
        /// <param name="convertFunction"> Функция для преобразования ячейки к нужному типу (null для типов, поддерживающих явное приведение) </param>
        /// <returns> Значение ячейки, либо дефолтное значение (ячейка не найдена), либо значение-флаг ошибки запроса </returns>
        public Result<T> SelectValue<T>(string query, Func<object, T> convertFunction = null)
        {
            if (String.IsNullOrWhiteSpace(query)) throw new ArgumentNullException();
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        var result = cmd.ExecuteScalar();
                        var value = (convertFunction == null) ? (T) result : convertFunction(result);
                        return Result.Ok(value);
                    }
                }
            }
            catch (Exception e)
            {
                return Result.Fail<T>("Ошибка запроса на выборку одиночного значения!", e);
            }
        }

        /// <summary> Запрос на выборку значения ячейки из базе данных без сокрытия ошибки дефолтным значением </summary>
        /// <typeparam name="T"> Тип возвращаемого значения </typeparam>
        /// <param name="query"> Запрос на получение значения из ячейки таблицы в БД </param>
        /// <param name="notFoundValue"> Возвращаемое значение в случае, если запрос вернет пустую таблицу (фильтр вернул пустой множество в запросе) </param>
        /// <param name="convertFunction"> Функция для преобразования ячейки к нужному типу (null для типов, поддерживающих явное приведение) </param>
        /// <returns> Значение ячейки, либо дефолтное значение (ячейка не найдена), либо значение-флаг ошибки запроса </returns>
        public Result<T> SelectRealValue<T>(string query, T notFoundValue, Func<object, T> convertFunction = null)
        {
            var result = SelectValue(query, convertFunction);
            if (result.IsFailured && (result.Error is InvalidCastException || result.Error is NullReferenceException))
            {
                return Result.Ok(notFoundValue);
            }

            return result;
        }

        /// <summary> Запрос на выборку значения ячейки из базе данных без сокрытия ошибки дефолтным значением </summary>
        /// <typeparam name="T"> Тип возвращаемого значения </typeparam>
        /// <param name="query"> Запрос на получение значения из ячейки таблицы в БД </param>
        /// <param name="notFoundValue"> Возвращаемое значение в случае, если запрос вернет пустую таблицу (фильтр вернул пустой множество в запросе) </param>
        /// <param name="queryErrorValue"> Возвращаемое значение в случае, если запрос завершился ошибкой (неверный синтаксис, недоступность БД и т.п.) </param>
        /// <param name="convertFunction"> Функция для преобразования ячейки к нужному типу (null для типов, поддерживающих явное приведение) </param>
        /// <returns> Значение ячейки, либо дефолтное значение (ячейка не найдена), либо значение-флаг ошибки запроса </returns>
        public T SelectRealValue<T>(string query, T notFoundValue, T queryErrorValue, Func<object, T> convertFunction = null)
        {
            var result = SelectRealValue(query, notFoundValue, convertFunction);
            return result.IsFailured ? queryErrorValue : result.Value;
        }
    }
}