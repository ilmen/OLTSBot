using Ninject;
using Ninject.Modules;
using StreamForRead.Classes;
using StreamForRead.Models;
using Telegram.Bot;

namespace StreamForRead.IoC
{
    public class CommonModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBotSettingsFactory>()
                .ToMethod(_ => new BotSettingsFactoryFromJson());

            Bind<TelegramBotFactory>()
                .ToSelf()
                .InSingletonScope();

            Bind<ITelegramBotClient>()
                .ToMethod(args =>
                {
                    var result = args.Kernel.Get<TelegramBotFactory>().Create();
                    result.ThrowIfFailure();
                    return result.Value;
                });
        }
    }
}