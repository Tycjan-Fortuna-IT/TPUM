namespace ClientServer.Shared.Logic.API
{
    public interface IHeroDataTransferObject
    {
        public abstract Guid Id { get; }

        public abstract string Name { get; set; }

        public abstract float Gold { get; set; }

        public abstract IInventoryDataTransferObject Inventory { get; set; }
    }
}
