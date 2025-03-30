namespace Logic.API
{
    public interface IHeroDataTransferObject
    {
        public abstract Guid Id { get; }

        public abstract string Name { get; }

        public abstract float Gold { get; }

        public abstract IInventoryDataTransferObject Inventory { get; }
    }
}
