using ClientServer.Shared.Logic.API;
using Server.Logic.API;

namespace Server.Logic.Implementation
{
    internal class MaintenanceTracker : IMaintenanceTracker
    {
        private readonly List<IObserver<IHeroDataTransferObject>> _observers;

        public MaintenanceTracker()
        {
            _observers = new List<IObserver<IHeroDataTransferObject>>();
        }

        public IDisposable Subscribe(IObserver<IHeroDataTransferObject> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubscriber(_observers, observer);
        }

        public void Track(IHeroDataTransferObject hero)
        {
            foreach (IObserver<IHeroDataTransferObject> o in _observers)
            {
                if (hero == null)
                {
                    o.OnError(new Exception("TrackHero is null"));
                }
                else
                {
                    o.OnNext(hero);
                }
            }
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<IHeroDataTransferObject>> _observers;
            private readonly IObserver<IHeroDataTransferObject> _observer;

            public Unsubscriber(List<IObserver<IHeroDataTransferObject>> observers, IObserver<IHeroDataTransferObject> observer)
            {
                _observer = observer;
                _observers = observers;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
