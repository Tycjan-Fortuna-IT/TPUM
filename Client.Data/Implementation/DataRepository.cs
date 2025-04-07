using Shared.Data.API;

namespace Client.Data.Implementation
{
    internal class DataRepository : IDataRepository
    {
        private readonly IDataContext _context;

        private readonly object _heroesLock = new object();
        private readonly object _inventoryLock = new object();
        private readonly object _itemsLock = new object();
        private readonly object _ordersLock = new object();

        public DataRepository(IDataContext context)
        {
            this._context = context;
        }

        public IEnumerable<IHero> GetAllHeroes()
        {
            lock (_heroesLock)
            {
                return _context.Heroes.Values.ToList();
            }
        }

        public IHero? GetHero(Guid id)
        {
            lock (_heroesLock)
            {
                if (_context.Heroes.ContainsKey(id))
                {
                    return _context.Heroes[id];
                }

                return null;
            }
        }

        public void AddHero(IHero hero)
        {
            lock (_heroesLock)
            {
                _context.Heroes[hero.Id] = hero;
            }
        }

        public bool RemoveHeroById(Guid id)
        {
            lock (_heroesLock)
            {
                if (_context.Heroes.ContainsKey(id))
                {
                    _context.Heroes.Remove(id);
                    return true;
                }

                return false;
            }
        }

        public bool RemoveHero(IHero hero)
        {
            lock (_heroesLock)
            {
                if (_context.Heroes.ContainsKey(hero.Id))
                {
                    _context.Heroes.Remove(hero.Id);
                    return true;
                }

                return false;
            }
        }

        public bool UpdateHero(Guid id, IHero hero)
        {
            lock (_heroesLock)
            {
                if (_context.Heroes.ContainsKey(id))
                {
                    _context.Heroes[id] = hero;
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<IInventory> GetAllInventories()
        {
            lock (_inventoryLock)
            {
                return _context.Inventories.Values.ToList();
            }
        }

        public IInventory? GetInventory(Guid id)
        {
            lock (_inventoryLock)
            {
                if (_context.Inventories.ContainsKey(id))
                {
                    return _context.Inventories[id];
                }

                return null;
            }
        }

        public void AddInventory(IInventory inventory)
        {
            lock (_inventoryLock)
            {
                _context.Inventories[inventory.Id] = inventory;
            }
        }

        public bool RemoveInventoryById(Guid id)
        {
            lock (_inventoryLock)
            {
                if (_context.Inventories.ContainsKey(id))
                {
                    _context.Inventories.Remove(id);
                    return true;
                }

                return false;
            }
        }

        public bool RemoveInventory(IInventory inventory)
        {
            lock (_inventoryLock)
            {
                if (_context.Inventories.ContainsKey(inventory.Id))
                {
                    _context.Inventories.Remove(inventory.Id);
                    return true;
                }

                return false;
            }
        }

        public bool UpdateInventory(Guid id, IInventory inventory)
        {
            lock (_inventoryLock)
            {
                if (_context.Inventories.ContainsKey(id))
                {
                    _context.Inventories[id] = inventory;
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<IItem> GetAllItems()
        {
            lock (_itemsLock)
            {
                return _context.Items.Values.ToList();
            }
        }

        public IItem? GetItem(Guid id)
        {
            lock (_itemsLock)
            {
                if (_context.Items.ContainsKey(id))
                {
                    return _context.Items[id];
                }

                return null;
            }
        }

        public void AddItem(IItem item)
        {
            lock (_itemsLock)
            {
                _context.Items[item.Id] = item;
            }
        }

        public bool RemoveItemById(Guid id)
        {
            lock (_itemsLock)
            {
                if (_context.Items.ContainsKey(id))
                {
                    _context.Items.Remove(id);
                    return true;
                }

                return false;
            }
        }

        public bool RemoveItem(IItem item)
        {
            lock (_itemsLock)
            {
                if (_context.Items.ContainsKey(item.Id))
                {
                    _context.Items.Remove(item.Id);
                    return true;
                }

                return false;
            }
        }

        public bool UpdateItem(Guid id, IItem item)
        {
            lock (_itemsLock)
            {
                if (_context.Items.ContainsKey(id))
                {
                    _context.Items[id] = item;
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<IOrder> GetAllOrders()
        {
            lock (_ordersLock)
            {
                return _context.Orders.Values.ToList();
            }
        }

        public IOrder? GetOrder(Guid id)
        {
            lock (_ordersLock)
            {
                if (_context.Orders.ContainsKey(id))
                {
                    return _context.Orders[id];
                }

                return null;
            }
        }

        public void AddOrder(IOrder order)
        {
            lock (_ordersLock)
            {
                _context.Orders[order.Id] = order;
            }
        }

        public bool RemoveOrderById(Guid id)
        {
            lock (_ordersLock)
            {
                if (_context.Orders.ContainsKey(id))
                {
                    _context.Orders.Remove(id);
                    return true;
                }

                return false;
            }
        }

        public bool RemoveOrder(IOrder order)
        {
            lock (_ordersLock)
            {
                if (_context.Orders.ContainsKey(order.Id))
                {
                    _context.Orders.Remove(order.Id);
                    return true;
                }

                return false;
            }
        }

        public bool UpdateOrder(Guid id, IOrder order)
        {
            lock (_ordersLock)
            {
                if (_context.Orders.ContainsKey(id))
                {
                    _context.Orders[id] = order;
                    return true;
                }

                return false;
            }
        }
    }
}
