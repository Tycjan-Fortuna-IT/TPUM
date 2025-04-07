namespace ClientServer.Shared.Data.API
{
    public interface IDataContext
    {
        public abstract Dictionary<Guid, IHero> Heroes { get; }

        public abstract Dictionary<Guid, IItem> Items { get; }

        public abstract Dictionary<Guid, IInventory> Inventories { get; }

        public abstract Dictionary<Guid, IOrder> Orders { get; }
    }
}
