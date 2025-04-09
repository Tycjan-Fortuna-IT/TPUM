using ClientServer.Shared.Logic.API;

namespace Server.Logic.API
{
    public interface IMaintenanceReporter : IObserver<IHeroDataTransferObject>
    {
        void Subscribe(IObservable<IHeroDataTransferObject> provider, Action onComplete, Action<Exception> onError, Action<IHeroDataTransferObject> onNext);
        void Unsubscribe();
    }
}