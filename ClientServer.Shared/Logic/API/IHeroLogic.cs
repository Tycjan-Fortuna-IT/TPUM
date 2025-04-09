namespace ClientServer.Shared.Logic.API
{
    public interface IHeroLogic
    {
        public abstract IEnumerable<IHeroDataTransferObject> GetAll();

        public abstract IHeroDataTransferObject? Get(Guid id);

        public abstract void Add(IHeroDataTransferObject item);

        public abstract bool RemoveById(Guid id);

        public abstract bool Remove(IHeroDataTransferObject item);

        public abstract bool Update(Guid id, IHeroDataTransferObject item);

        public abstract void PeriodicItemMaintenanceDeduction();

        public abstract void DeduceMaintenanceCost(IHeroDataTransferObject hero);
    }
}
