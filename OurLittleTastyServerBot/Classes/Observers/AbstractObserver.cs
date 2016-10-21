using System;
using OurLittleTastyServerBot.Classes.Logging;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public abstract class AbstractObserver<T> : IObserver<T>
    {
        protected readonly Logger _logger;

        protected AbstractObserver()
        {
            _logger = Logger.Singleton.GetInstance();
        }

        public abstract void OnNext(T value);

        public void OnError(Exception error)
        {
            _logger.WriteError("AbstractObserver - провайдер вернул ошибку!", error);
        }

        public void OnCompleted()
        {
            _logger.WriteDebug("AbstractObserver - отписан от обновлений");
        }
    }
}