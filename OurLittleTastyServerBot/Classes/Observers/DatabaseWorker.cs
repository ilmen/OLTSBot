using System;
using System.Linq;
using OurLittleTastyServerBot.Classes.Db.Models;
using OurLittleTastyServerBot.Classes.Db.Repositories;
using OurLittleTastyServerBot.Classes.Events;
using OurLittleTastyServerBot.Classes.Telegram.Models;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public class DatabaseWorker : AbstractObserver<EventArgs>
    {
        private readonly EventBroker _broker;
        private readonly AbstractRepository<UpdateRecord> _repository;

        public DatabaseWorker(EventBroker broker, AbstractRepository<UpdateRecord> repository)
        {
            _broker = broker;
            _repository = repository;
        }

        public override void OnNext(EventArgs value)
        {
            var updateEvArg = value as UpdateEventArgs;
            if (updateEvArg == null)
            {
                _logger.WriteError("DatabaseSaver - провайдер вернул некорректный тип оповещения: " + value.GetType());
                return;
            }

            var recordResult = Convert(updateEvArg.Data);
            if (recordResult.IsFailured)
            {
                _logger.WriteError("DatabaseSaver - ошибка конвертации в объект UpdateRecord: " + recordResult.ErrorText);
                return;
            }
                
            if (_repository.GetAll().Any(x => x.UpdateOuterId == recordResult.Value.UpdateOuterId)) return;

            var insertResult = _repository.Insert(recordResult.Value);
            if (insertResult.IsFailured)
            {
                _logger.WriteError("DatabaseSaver - ошибка добавления записи в репозиторий: " + insertResult.ErrorText);
                return;
            }

            _logger.WriteDebug("DatabaseSaver - запись добавлена в репозиторий. Id=" + insertResult.Value.Id + ". Text=" + insertResult.Value.Text);

            var evArgs = new UpdateRecordEventArgs(insertResult.Value);
            _broker.Publish(evArgs);
        }

        private static Result<UpdateRecord> Convert(Update value)
        {
            try
            {
                var insertTime = DateTime.Now;
                var sendTime = UnixDateTime.FromUnixFormat(value.message.UtcDateTimeInUnitFormat);

                var result = UpdateRecord.Factory.CreateEmpty(value.update_id, value.message.message_id, sendTime, insertTime, value.message.text, value.message.chat.id, value.message.from.id, value.message.from.first_name);
                    
                return result;
            }
            catch (Exception e)
            {
                return Result.Fail<UpdateRecord>("Ошибка конвертации", e);
            }
        }
    }
}