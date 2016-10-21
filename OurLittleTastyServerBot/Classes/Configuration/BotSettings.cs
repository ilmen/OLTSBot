using System;

namespace OurLittleTastyServerBot.Classes.Configuration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BotSettings
    {
        public string TelegramApiUrlPattern
        { get; private set; }

        public string BotToken
        { get; private set; }

        public BotSettings(string telegramApiUrlPattern, string botToken)
        {
            if (telegramApiUrlPattern == null) throw new ArgumentNullException("telegramApiUrlPattern");
            TelegramApiUrlPattern = telegramApiUrlPattern;

            if (botToken == null) throw new ArgumentNullException("botToken");
            BotToken = botToken;
        }
    }
}