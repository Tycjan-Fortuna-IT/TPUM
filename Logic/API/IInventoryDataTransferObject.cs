namespace Logic.API
{
    public interface IInventoryDataTransferObject
    {
        public abstract Guid Id { get; }
        public abstract int Capacity { get; }

        public abstract IEnumerable<IItemDataTransferObject> Items { get; }
    }
}
