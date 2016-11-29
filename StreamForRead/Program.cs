using System;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StreamForRead
{
    public static class Program
    {
        private static ITelegramBotClient _bot;

        public static void Main(string[] args)
        {
            var token = Properties.Settings.Default.BotToken.Trim();
            if (String.IsNullOrEmpty(token))
            {
                Console.WriteLine("Getting token error. Check config file!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Bot starting...");

            StartBot(token);

            Console.ReadLine();
        }

        private static async void StartBot(string token)
        {
            // see: https://github.com/MrRoundRobin/telegram.bot

            _bot = new TelegramBotClient(token);
            
            var info = await _bot.GetMeAsync();
            Console.WriteLine("Bot \"{0}\" started.", info.FirstName);

            // remove web hook
            await _bot.SetWebhookAsync();

            while (true)
            {
                try
                {
                    var updates = await _bot.GetUpdatesAsync();
                    foreach (var update in updates)
                    {
                        Work(update);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void Work(Update update)
        {
            // Обработать отдельно /start - показать справку-приветствие
            /* API
             * полученные сообщения сохраняем
             * по запросу очищаем сохраненные сообщения
             * по запросу получаем все сохраненные сообщения (постраничный просмотр изменением сообщения)
             *  можно отображать простой список, краткий, а после щелчка по пункту сообщения высылаем полную версию сообщения
             */

            string answer;
            switch (update.Message.Text.ToLower())
            {
                default:
                    answer = "Вы сказали: " + update.Message.Text;
                    break;
            }
            if (!String.IsNullOrEmpty(answer))
            {
                _bot.SendTextMessageAsync(update.Message.Chat.Id, answer);
            }
        }
    }
}
