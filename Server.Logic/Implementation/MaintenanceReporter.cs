using ClientServer.Shared.Logic.API;
using Server.Logic.API;

namespace Server.Logic.Implementation
{
    internal class MaintenanceReporter : IMaintenanceReporter
    {
        private IDisposable _unsubscriber = null!;
        private Action _onComplete = null!;
        private Action<IHeroDataTransferObject> _onNext = null!;
        private Action<Exception> _onError = null!;

        public void Subscribe(IObservable<IHeroDataTransferObject> provider, Action onComplete, Action<Exception> onError, Action<IHeroDataTransferObject> onNext)
        {
            _onComplete = onComplete;
            _onError = onError;
            _onNext = onNext;

            if (provider != null)
            {
                _unsubscriber = provider.Subscribe(this);
            }
        }
        public void Unsubscribe()
        {
            _unsubscriber?.Dispose();
        }

        public void OnCompleted()
        {
            _onComplete?.Invoke();

            this.Unsubscribe();
        }

        public void OnError(Exception error)
        {
            _onError?.Invoke(error);
        }

        public void OnNext(IHeroDataTransferObject value)
        {
            _onNext?.Invoke(value);
        }
    }
}
