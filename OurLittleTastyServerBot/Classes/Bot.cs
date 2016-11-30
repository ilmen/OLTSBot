using System;
using System.Reactive.Linq;
using System.Threading;
using Newtonsoft.Json;
using OurLittleTastyServerBot.Classes.Configuration;
using OurLittleTastyServerBot.Classes.Db;
using OurLittleTastyServerBot.Classes.Db.Models;
using OurLittleTastyServerBot.Classes.Db.Repositories;
using OurLittleTastyServerBot.Classes.Events;
using OurLittleTastyServerBot.Classes.Observers;
using OurLittleTastyServerBot.Classes.Telegram.Models;

namespace OurLittleTastyServerBot.Classes
{
    public class Bot
    {
        //public static Xakpc.TelegramBot.Client.ITelegramBotApiClient bot;

        private readonly BotSettings _botSettings;
        private readonly EventBroker _eventBroker;
        private readonly SqliteDatabase _db;
        private readonly AbstractRepository<UpdateRecord> _updateRepository;
        private readonly AbstractRepository<DialogRecord> _dialogRepository;

        public Bot(BotSettings botSettings, EventBroker eventBroker)
        {
            _botSettings = botSettings;
            _eventBroker = eventBroker;

            _db = new SqliteDatabase("database3.db");
            _updateRepository = new UpdateRecordRepositoryInSQLite(_db);
            _dialogRepository = new DialogRepositoryInMemory();

            InitializeEventBroker();
        }

        private void InitializeEventBroker()
        {
            _eventBroker.OfType<UpdateEventArgs>().Subscribe(new DatabaseWorker(_eventBroker, _updateRepository));
            _eventBroker.OfType<UpdateRecordEventArgs>().Subscribe(new CommandParser(_eventBroker));
            _eventBroker.OfType<MessageEventArgs>().Subscribe(new MessageWorker(_botSettings));
            _eventBroker.OfType<CommandEventArgs>().Where(x => x.Command == EnCommand.WeatherRequst).Subscribe(new WeatherWorker(_eventBroker));
            _eventBroker.OfType<CommandEventArgs>().Where(x => x.Command == EnCommand.Text).Subscribe(new EchoTextWorker(_eventBroker));
            _eventBroker.OfType<CommandEventArgs>().Where(x => x.Command == EnCommand.AddSimpleCostRequest).Subscribe(new SimpleAddingCostWorker(_eventBroker));    //, _dialogRepository));
        }

        public void Start()
        {
            while (true)
            {
                try
                {
                    //bot = new Xakpc.TelegramBot.Client.TelegramBotApiClient(_botSettings.BotToken);

                    var getUpdatesResult = GetUpdates();
                    getUpdatesResult.ThrowIfFailure();

                    var updates = getUpdatesResult.Value.Result;

                    foreach (var update in updates)
                    {
                        var updateArgs = new UpdateEventArgs(update);
                        _eventBroker.Publish(updateArgs);
                    }

                    Thread.Sleep(1000);     // нельзя отправлять больше N запросов к API в секунду (перестраховываемся с 1 секудой, пока этой скрости достаточно)
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private Result<JsonResult<Update[]>> GetUpdates()
        {
            var url = string.Format(_botSettings.TelegramApiUrlPattern, _botSettings.BotToken, "getUpdates");
            var result = RequestHelper.Get(url);
            if (result.IsFailured) return result.FailCastTo<JsonResult<Update[]>>();

            var json = result.Value;
            var updates = JsonConvert.DeserializeObject<JsonResult<Update[]>>(json);
            return Result.Ok(updates);
        }
    }
}