using Client.Presentation.Model.API;

namespace Client.Presentation.ViewModel.Tests
{
    internal class DummyHeroModel : IHeroModel
    {
        public Guid Id { get; }
        public string Name { get; }
        public float Gold { get; }
        public IInventoryModel Inventory { get; }

        public DummyHeroModel(Guid id, string name, float gold, IInventoryModel inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }

    internal class DummyInventoryModel : IInventoryModel
    {
        public Guid Id { get; }
        public int Capacity { get; }
        public IEnumerable<IItemModel> Items { get; }

        public DummyInventoryModel(Guid id, int capacity, IEnumerable<IItemModel> items)
        {
            Id = id;
            Capacity = capacity;
            Items = items ?? Enumerable.Empty<IItemModel>();
        }
    }

    internal class DummyItemModel : IItemModel
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        public DummyItemModel(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
    }

    internal class DummyOrderModel : IOrderModel
    {
        public Guid Id { get; }
        public IHeroModel Buyer { get; }
        public IEnumerable<IItemModel> ItemsToBuy { get; }

        public DummyOrderModel(Guid id, IHeroModel buyer, IEnumerable<IItemModel> itemsToBuy)
        {
            Id = id;
            Buyer = buyer;
            ItemsToBuy = itemsToBuy?.ToList() ?? new List<IItemModel>();
        }
    }

    internal class DummyHeroModelService : IHeroModelService
    {
        private readonly Dictionary<Guid, IHeroModel> _heroes = new();
        private readonly IInventoryModelService _inventoryService;

        public DummyHeroModelService(IInventoryModelService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public void AddHero(Guid id, string name, float gold, Guid inventoryId)
        {
            IInventoryModel? inventory = _inventoryService.GetInventory(inventoryId);
            if (inventory == null) throw new ArgumentException("Invalid inventory ID");
            _heroes[id] = new DummyHeroModel(id, name, gold, inventory);
        }

        public IEnumerable<IHeroModel> GetAllHeroes() => _heroes.Values;

        public IHeroModel? GetHero(Guid id) => _heroes.GetValueOrDefault(id);

        public bool RemoveHero(Guid id) => _heroes.Remove(id);

        public bool UpdateHero(Guid id, string name, float gold, Guid inventoryId)
        {
            if (!_heroes.ContainsKey(id)) return false;
            IInventoryModel? inventory = _inventoryService.GetInventory(inventoryId);
            if (inventory == null) return false;
            _heroes[id] = new DummyHeroModel(id, name, gold, inventory);
            return true;
        }

        public void TriggerPeriodicItemMaintenanceDeduction()
        {
            foreach (IHeroModel hero in _heroes.Values)
            {
                float totalCost = hero.Inventory.Items.Sum(item => item.MaintenanceCost);
                _heroes[hero.Id] = new DummyHeroModel(hero.Id, hero.Name, Math.Max(0, hero.Gold - totalCost), hero.Inventory);
            }
        }
    }

    internal class DummyInventoryModelService : IInventoryModelService
    {
        private readonly Dictionary<Guid, IInventoryModel> _inventories = new();

        public void Add(Guid id, int capacity)
        {
            _inventories[id] = new DummyInventoryModel(id, capacity, new List<IItemModel>());
        }

        public IEnumerable<IInventoryModel> GetAllInventories() => _inventories.Values;

        public IInventoryModel? GetInventory(Guid id) => _inventories.GetValueOrDefault(id);
    }

    internal class DummyItemModelService : IItemModelService
    {
        private readonly Dictionary<Guid, IItemModel> _items = new();

        public void AddItem(Guid id, string name, int price, int maintenanceCost)
        {
            _items[id] = new DummyItemModel(id, name, price, maintenanceCost);
        }

        public IEnumerable<IItemModel> GetAllItems() => _items.Values;

        public IItemModel? GetItem(Guid id) => _items.GetValueOrDefault(id);

        public bool RemoveItem(Guid id) => _items.Remove(id);

        public bool UpdateItem(Guid id, string name, int price, int maintenanceCost)
        {
            if (!_items.ContainsKey(id)) return false;
            _items[id] = new DummyItemModel(id, name, price, maintenanceCost);
            return true;
        }
    }

    internal class DummyOrderModelService : IOrderModelService
    {
        private readonly Dictionary<Guid, IOrderModel> _orders = new();
        private readonly IHeroModelService _heroService;
        private readonly IItemModelService _itemService;

        public DummyOrderModelService(IHeroModelService heroService, IItemModelService itemService)
        {
            _heroService = heroService;
            _itemService = itemService;
        }

        public void AddOrder(Guid id, Guid buyerId, IEnumerable<Guid> itemIds)
        {
            IHeroModel? buyer = _heroService.GetHero(buyerId);
            if (buyer == null) throw new ArgumentException("Invalid buyer ID");

            List<IItemModel?> itemsToBuy = itemIds.Select(id => _itemService.GetItem(id)).Where(item => item != null).ToList();
            if (!itemsToBuy.Any()) throw new ArgumentException("No valid items to buy");

            _orders[id] = new DummyOrderModel(id, buyer, itemsToBuy!);
        }

        public IEnumerable<IOrderModel> GetAllOrders() => _orders.Values;

        public IOrderModel? GetOrder(Guid id) => _orders.GetValueOrDefault(id);

        public bool RemoveOrder(Guid id) => _orders.Remove(id);

        public void TriggerPeriodicOrderProcessing()
        {
            foreach (IOrderModel order in _orders.Values.ToList())
            {
                float totalCost = order.ItemsToBuy.Sum(item => item.Price);
                if (order.Buyer.Gold >= totalCost)
                {
                    _heroService.UpdateHero(order.Buyer.Id, order.Buyer.Name, order.Buyer.Gold - totalCost, order.Buyer.Inventory.Id);
                    _orders.Remove(order.Id);
                }
            }
        }
    }
}