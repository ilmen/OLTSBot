using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OurLittleTastyServerBot.Classes.Events
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class EventBroker : IObservable<EventArgs>
    {
        private ImmutableHashSet<Subscribtion> _subscribtions = ImmutableHashSet<Subscribtion>.Empty;

        public void Publish<T>(T args)
            where T : EventArgs
        {
            foreach (var subscribtion in _subscribtions)
            {
                subscribtion.Sunscriber.OnNext(args);
            }
        }

        public IDisposable Subscribe(IObserver<EventArgs> observer)
        {
            var subscribtion = new Subscribtion(this, observer);
            if (_subscribtions.All(x => x.Sunscriber != observer))
            {
                _subscribtions = _subscribtions.Add(subscribtion);
            }
            return subscribtion;
        }

        public void Unsubscribe(IObserver<EventArgs> subscriber)
        {
            var toRemove = _subscribtions.SingleOrDefault(x => x.Sunscriber == subscriber);
            if (toRemove != null)
            {
                _subscribtions = _subscribtions.Remove(toRemove);
            }
        }

        private class Subscribtion : IDisposable
        {
            private readonly EventBroker _broker;

            public Subscribtion(EventBroker broker, IObserver<EventArgs> sunscriber)
            {
                Sunscriber = sunscriber;
                _broker = broker;
            }

            public IObserver<EventArgs> Sunscriber { get; private set; }

            public void Dispose()
            {
                _broker.Unsubscribe(Sunscriber);
            }
        }
    }

    #region Старая реализация, на основе List<Subscribtion>

    //[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    //public class EventBroker : IObservable<EventArgs>
    //{
    //    private readonly List<Subscribtion> _subscribtions = new List<Subscribtion>();
    //    private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

    //    public void Publish<T>(T args)
    //        where T : EventArgs
    //    {
    //        _locker.EnterReadLock();
    //        try
    //        {
    //            foreach (var subscribtion in _subscribtions)
    //            {
    //                subscribtion.Sunscriber.OnNext(args);
    //            }
    //        }
    //        finally
    //        {
    //            _locker.ExitReadLock();
    //        }
    //    }

    //    public IDisposable Subscribe(IObserver<EventArgs> observer)
    //    {
    //        var subscribtion = new Subscribtion(this, observer);
    //        _locker.EnterUpgradeableReadLock();
    //        try
    //        {
    //            if (_subscribtions.All(x => x.Sunscriber != observer))
    //            {
    //                _locker.EnterWriteLock();
    //                try
    //                {
    //                    _subscribtions.Add(subscribtion);
    //                }
    //                finally
    //                {
    //                    _locker.ExitWriteLock();
    //                }
    //            }
    //        }
    //        finally
    //        {
    //            _locker.ExitUpgradeableReadLock();
    //        }
    //        return subscribtion;
    //    }

    //    public void Unsubscribe(IObserver<EventArgs> subscriber)
    //    {
    //        _locker.EnterWriteLock();
    //        try
    //        {
    //            _subscribtions.RemoveAll(x => x.Sunscriber == subscriber);
    //        }
    //        finally
    //        {
    //            _locker.ExitWriteLock();
    //        }
    //    }

    //    private class Subscribtion : IDisposable
    //    {
    //        private readonly EventBroker _broker;

    //        public Subscribtion(EventBroker broker, IObserver<EventArgs> sunscriber)
    //        {
    //            Sunscriber = sunscriber;
    //            _broker = broker;
    //        }

    //        public IObserver<EventArgs> Sunscriber { get; private set; }

    //        public void Dispose()
    //        {
    //            _broker.Unsubscribe(Sunscriber);
    //        }
    //    }
    //}

    #endregion
}