using System.Diagnostics.CodeAnalysis;
using OurLittleTastyServerBot.Classes.Events;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public class CommandParser : AbstractObserver<UpdateRecordEventArgs>
    {
        private readonly EventBroker _broker;

        public CommandParser(EventBroker broker)
        {
            _broker = broker;
        }

        public override void OnNext(UpdateRecordEventArgs value)
        {
            var text = value.Record.Text;

            var cmd = GetCommand(text);
            var evArgs = new CommandEventArgs(cmd, value.Record);

            _logger.WriteDebug("CommandParser: Тип команды: " + cmd + ". Текст: " + text);

            _broker.Publish(evArgs);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "InvertIf")]
        private static EnCommand GetCommand(string text)
        {
            var lowerText = text.ToLower();

            if (lowerText == @"/weather") return EnCommand.WeatherRequst;

            if (lowerText.StartsWith("/addcost"))
            {
                const string REGEX = @"^/addcost .* \d*$";
                if (System.Text.RegularExpressions.Regex.IsMatch(lowerText, REGEX)) return EnCommand.AddSimpleCostRequest;
            }

            return EnCommand.Text;
        }
    }

    public enum EnCommand
    {
        Text = 0,
        WeatherRequst = 1,
        AddSimpleCostRequest = 2
    }
}