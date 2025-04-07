namespace ClientServer.Shared.Data.API
{
    public interface IInventory : IIdentifiable
    {
        public abstract int Capacity { get; }

        public abstract IEnumerable<IItem> Items { get; }
    }
}
