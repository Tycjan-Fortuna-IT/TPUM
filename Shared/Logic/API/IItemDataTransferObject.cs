namespace ClientServer.Shared.Logic.API
{
    public interface IItemDataTransferObject
    {
        public abstract Guid Id { get; }

        public abstract string Name { get; }

        public abstract int Price { get; }

        public abstract int MaintenanceCost { get; }
    }
}
