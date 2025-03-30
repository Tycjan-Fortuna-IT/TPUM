using Data.API;

namespace Logic.Tests
{
    internal class DummyDataRepository : IDataRepository
    {
        private readonly Dictionary<Guid, IHero> _heroes = new Dictionary<Guid, IHero>();
        private readonly Dictionary<Guid, IItem> _items = new Dictionary<Guid, IItem>();
        private readonly Dictionary<Guid, IInventory> _inventories = new Dictionary<Guid, IInventory>();
        private readonly Dictionary<Guid, IOrder> _orders = new Dictionary<Guid, IOrder>();

        private readonly object _heroesLock = new object();
        private readonly object _inventoryLock = new object();
        private readonly object _itemsLock = new object();
        private readonly object _ordersLock = new object();

        public IEnumerable<IHero> GetAllHeroes()
        {
            lock (_heroesLock)
            {
                return _heroes.Values.ToList();
            }
        }

        public IHero? GetHero(Guid id)
        {
            lock (_heroesLock)
            {
                if (_heroes.ContainsKey(id))
                {
                    return _heroes[id];
                }

                return null;
            }
        }

        public void AddHero(IHero hero)
        {
            lock (_heroesLock)
            {
                _heroes[hero.Id] = hero;
            }
        }

        public bool RemoveHeroById(Guid id)
        {
            lock (_heroesLock)
            {
                if (_heroes.ContainsKey(id))
                {
                    _heroes.Remove(id);
                    return true;
                }

                return false;
            }
        }

        public bool RemoveHero(IHero hero)
        {
            lock (_heroesLock)
            {
                if (_heroes.ContainsKey(hero.Id))
                {
                    _heroes.Remove(hero.Id);
                    return true;
                }

                return false;
            }
        }

        public bool UpdateHero(Guid id, IHero hero)
        {
            lock (_heroesLock)
            {
                if (_heroes.ContainsKey(id))
                {
                    _heroes[id] = hero;
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<IInventory> GetAllInventories()
        {
            lock (_inventoryLock)
            {
                return _inventories.Values.ToList();
            }
        }

        public IInventory? GetInventory(Guid id)
        {
            lock (_inventoryLock)
            {
                if (_inventories.ContainsKey(id))
                {
                    return _inventories[id];
                }

                return null;
            }
        }

        public void AddInventory(IInventory inventory)
        {
            lock (_inventoryLock)
            {
                _inventories[inventory.Id] = inventory;
            }
        }

        public bool RemoveInventoryById(Guid id)
        {
            lock (_inventoryLock)
            {
                if (_inventories.ContainsKey(id))
                {
                    _inventories.Remove(id);
                    return true;
                }

                return false;
            }
        }

        public bool RemoveInventory(IInventory inventory)
        {
            lock (_inventoryLock)
            {
                if (_inventories.ContainsKey(inventory.Id))
                {
                    _inventories.Remove(inventory.Id);
                    return true;
                }

                return false;
            }
        }

        public bool UpdateInventory(Guid id, IInventory inventory)
        {
            lock (_inventoryLock)
            {
                if (_inventories.ContainsKey(id))
                {
                    _inventories[id] = inventory;
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<IItem> GetAllItems()
        {
            lock (_itemsLock)
            {
                return _items.Values.ToList();
            }
        }

        public IItem? GetItem(Guid id)
        {
            lock (_itemsLock)
            {
                if (_items.ContainsKey(id))
                {
                    return _items[id];
                }

                return null;
            }
        }

        public void AddItem(IItem item)
        {
            lock (_itemsLock)
            {
                _items[item.Id] = item;
            }
        }

        public bool RemoveItemById(Guid id)
        {
            lock (_itemsLock)
            {
                if (_items.ContainsKey(id))
                {
                    _items.Remove(id);
                    return true;
                }

                return false;
            }
        }

        public bool RemoveItem(IItem item)
        {
            lock (_itemsLock)
            {
                if (_items.ContainsKey(item.Id))
                {
                    _items.Remove(item.Id);
                    return true;
                }

                return false;
            }
        }

        public bool UpdateItem(Guid id, IItem item)
        {
            lock (_itemsLock)
            {
                if (_items.ContainsKey(id))
                {
                    _items[id] = item;
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<IOrder> GetAllOrders()
        {
            lock (_ordersLock)
            {
                return _orders.Values.ToList();
            }
        }

        public IOrder? GetOrder(Guid id)
        {
            lock (_ordersLock)
            {
                if (_orders.ContainsKey(id))
                {
                    return _orders[id];
                }

                return null;
            }
        }

        public void AddOrder(IOrder order)
        {
            lock (_ordersLock)
            {
                _orders[order.Id] = order;
            }
        }

        public bool RemoveOrderById(Guid id)
        {
            lock (_ordersLock)
            {
                if (_orders.ContainsKey(id))
                {
                    _orders.Remove(id);
                    return true;
                }

                return false;
            }
        }

        public bool RemoveOrder(IOrder order)
        {
            lock (_ordersLock)
            {
                if (_orders.ContainsKey(order.Id))
                {
                    _orders.Remove(order.Id);
                    return true;
                }

                return false;
            }
        }

        public bool UpdateOrder(Guid id, IOrder order)
        {
            lock (_ordersLock)
            {
                if (_orders.ContainsKey(id))
                {
                    _orders[id] = order;
                    return true;
                }

                return false;
            }
        }
    }
}
