using System;
using System.Diagnostics.CodeAnalysis;
using StreamForRead.Models;
using Telegram.Bot;

namespace StreamForRead.Classes
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class TelegramBotFactory
    {
        private readonly IBotSettingsFactory _botSettingsFactory;

        public TelegramBotFactory(IBotSettingsFactory botSettingsFactory)
        {
            _botSettingsFactory = botSettingsFactory;
        }

        public Result<TelegramBotClient> Create()
        {
            var tokenResult = _botSettingsFactory.GetToken();
            tokenResult.ThrowIfFailure();
            var token = tokenResult.Value;

            try
            {
                var bot = new TelegramBotClient(token.Token);
                return Result.Ok(bot);
            }
            catch (Exception e)
            {
                return Result.Fail<TelegramBotClient>("Ошибка формирования бота: " + e.Message, e);
            }
        }
    }
}