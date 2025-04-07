namespace Shared.Logic.API
{
    public interface IInventoryLogic
    {
        public abstract IEnumerable<IInventoryDataTransferObject> GetAll();

        public abstract IInventoryDataTransferObject? Get(Guid id);

        public abstract void Add(IInventoryDataTransferObject item);

        public abstract bool RemoveById(Guid id);

        public abstract bool Remove(IInventoryDataTransferObject item);

        public abstract bool Update(Guid id, IInventoryDataTransferObject item);
    }
}
