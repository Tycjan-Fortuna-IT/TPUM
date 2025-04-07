using ClientServer.Shared.Data.API;

namespace Server.Data.Implementation
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
            Guid hero1Guid = Guid.NewGuid();
            _heroes.Add(hero1Guid, new Hero(hero1Guid, "Arthas the Fallen", 3000.0f, new Inventory(30)));

            Guid hero2Guid = Guid.NewGuid();
            _heroes.Add(hero2Guid, new Hero(hero2Guid, "Lyra the Shadowblade", 2200.0f, new Inventory(20)));

            Guid hero3Guid = Guid.NewGuid();
            _heroes.Add(hero3Guid, new Hero(hero3Guid, "Gorrok the Unbreakable", 5000.0f, new Inventory(40)));

            Guid hero4Guid = Guid.NewGuid();
            _heroes.Add(hero4Guid, new Hero(hero4Guid, "Zyra the Flameborn", 2800.0f, new Inventory(25)));

            Guid item1Guid = Guid.NewGuid();
            _items.Add(item1Guid, new Item(item1Guid, "Magic Sword of Flames", 150, 35));

            Guid item2Guid = Guid.NewGuid();
            _items.Add(item2Guid, new Item(item2Guid, "Ancient Shield of Aegis", 250, 50));

            Guid item3Guid = Guid.NewGuid();
            _items.Add(item3Guid, new Item(item3Guid, "Dagger of Eternal Night", 120, 20));

            Guid item4Guid = Guid.NewGuid();
            _items.Add(item4Guid, new Item(item4Guid, "Ring of Arcane Wisdom", 180, 10));

            Guid item5Guid = Guid.NewGuid();
            _items.Add(item5Guid, new Item(item5Guid, "Potion of Immortality", 500, 0));

            Guid item6Guid = Guid.NewGuid();
            _items.Add(item6Guid, new Item(item6Guid, "Bow of the Stormcaller", 200, 40));
        }
    }
}
