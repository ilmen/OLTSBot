using System;
using OurLittleTastyServerBot.Classes.Events;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public class WeatherWorker : CommandProcessor
    {
        public WeatherWorker(EventBroker broker) : base(broker, EnCommand.WeatherRequst)
        {
        }

        protected override void Process(CommandEventArgs value)
        {
            var weatherProvider = new WeatherProvider();
            var weather = weatherProvider.GetCurrentWeather("novosibirsk");
            var msg = "";
            if (weather.IsFailured)
            {
                msg = "Ошибка получения погоды!";
            }
            else
            {
                msg += "Погода:" + Environment.NewLine;
                msg += "Температура: " + weather.Value.Temperature + Environment.NewLine;
                msg += "Скорость ветра: " + weather.Value.WindSpeed + Environment.NewLine;
                msg += "Влажность: " + weather.Value.Humidity + Environment.NewLine;
                msg += "Солнце. Восход: " + weather.Value.Sunrise + ". Закат: " + weather.Value.Sunset + "." +
                       Environment.NewLine;
                msg += "В общем: " + weather.Value.SummaryDescription;
            }

            _logger.WriteDebug("WeatherWorker: Погода получена. ChatId: " + value.Record.ChatOuterId);

            var evArgs = new MessageEventArgs(msg, value.Record);
            _broker.Publish(evArgs);
        }
    }
}