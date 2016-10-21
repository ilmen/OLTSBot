using OurLittleTastyServerBot.Classes.Events;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public class EchoTextWorker : CommandProcessor
    {
        public EchoTextWorker(EventBroker broker) : base(broker, EnCommand.Text)
        {
        }

        protected override void Process(CommandEventArgs value)
        {
            _logger.WriteDebug("EchoTextWorker: ECHO " + value.Record.Text);

            var evArgs = new MessageEventArgs("Вы сказали: " + value.Record.Text, value.Record);
            _broker.Publish(evArgs);
        }
    }
}