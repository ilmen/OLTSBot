using System;
using Ninject;
using StreamForRead.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace StreamForRead
{
    public static class Program
    {
        private static readonly StandardKernel Kernel = new StandardKernel(new IoC.CommonModule());

        public static void Main(string[] args)
        {
            var settings = Kernel.Get<IBotSettingsFactory>();
            var checkTokenAvailability = settings.GetToken();
            if (checkTokenAvailability.IsFailured)
            {
                Console.WriteLine(checkTokenAvailability.ErrorText);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Bot starting...");

            StartBot();

            Console.ReadLine();
        }

        private static async void StartBot()
        {
            // see: https://github.com/MrRoundRobin/telegram.bot

            var bot = Kernel.Get<ITelegramBotClient>();

            var info = await bot.GetMeAsync();
            Console.WriteLine("Bot \"{0}\" started.", info.FirstName);

            // remove web hook
            await bot.SetWebhookAsync();

            while (true)
            {
                try
                {
                    var updates = await bot.GetUpdatesAsync();
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
                var bot = Kernel.Get<ITelegramBotClient>();
                bot.SendTextMessageAsync(update.Message.Chat.Id, answer);
            }
        }
    }
}
