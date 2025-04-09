using ClientServer.Shared.Logic.API;

namespace Server.Logic.API
{
    public interface IMaintenanceTracker : IObservable<IHeroDataTransferObject>
    {
        public void Track(IHeroDataTransferObject device);
    }
}
