namespace Logic.API
{
    public interface IItemLogic
    {
        public abstract IEnumerable<IItemDataTransferObject> GetAll();

        public abstract IItemDataTransferObject? Get(Guid id);

        public abstract void Add(IItemDataTransferObject item);

        public abstract bool RemoveById(Guid id);

        public abstract bool Remove(IItemDataTransferObject item);

        public abstract bool Update(Guid id, IItemDataTransferObject item);
    }
}
