using Data.API;

namespace Data.Implementation
{
    internal class DataContext : IDataContext
    {
        private readonly Dictionary<Guid, IHero> _heroes = new Dictionary<Guid, IHero>();
        private readonly Dictionary<Guid, IItem> _items = new Dictionary<Guid, IItem>();
        private readonly Dictionary<Guid, IInventory> _inventories = new Dictionary<Guid, IInventory>();
        private readonly Dictionary<Guid, IOrder> _orders = new Dictionary<Guid, IOrder>();

        public Dictionary<Guid, IHero> Heroes => _heroes;
        public Dictionary<Guid, IItem> Items => _items;
        public Dictionary<Guid, IInventory> Inventories => _inventories;
        public Dictionary<Guid, IOrder> Orders => _orders;
    }
}
