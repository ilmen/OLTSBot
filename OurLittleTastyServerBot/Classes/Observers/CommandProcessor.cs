using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OurLittleTastyServerBot.Classes.Events;

namespace OurLittleTastyServerBot.Classes.Observers
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class CommandProcessor : AbstractObserver<CommandEventArgs>
    {
        protected readonly EventBroker _broker;
        protected readonly EnCommand _expectedCommandType;

        protected CommandProcessor(EventBroker broker, EnCommand expectedCommandType)
        {
            _broker = broker;
            _expectedCommandType = expectedCommandType;
        }

        public override void OnNext(CommandEventArgs value)
        {
            if (value.Command != _expectedCommandType)
            {
                var errmsg = string.Format("Получен неожиданный тип команды! Ожидаемый: {0}, полученный: {1}!", _expectedCommandType, value.Command);
                _logger.WriteError(errmsg);
                if (Debugger.IsAttached) Debug.Assert(false, errmsg);
                return;
            }

            Process(value);
        }

        protected abstract void Process(CommandEventArgs value);
    }
}