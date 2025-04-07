namespace ClientServer.Shared.Logic.API
{
    public interface IOrderLogic
    {
        public abstract IEnumerable<IOrderDataTransferObject> GetAll();

        public abstract IOrderDataTransferObject? Get(Guid id);

        public abstract void Add(IOrderDataTransferObject item);

        public abstract bool RemoveById(Guid id);

        public abstract bool Remove(IOrderDataTransferObject item);

        public abstract bool Update(Guid id, IOrderDataTransferObject item);

        public abstract void PeriodicOrderProcessing();
    }
}
