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

        public DataContext()
        {
            // some initial data context state
            Guid hero1Guid = new Guid("ea8ba3bf-4dcd-4f6e-b698-8a81d41ff7f7");
            _heroes.Add(hero1Guid, new Hero(hero1Guid, "Hero 1", 2500.0f, new Inventory(25)));

            Guid item1Guid = new Guid("a93af587-96a2-4d0e-8b52-451ea9d14562");
            _items.Add(item1Guid, new Item(item1Guid, "Magic sword", 100, 25));
        }
    }
}
