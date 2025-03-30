using Data.API;

namespace Data.Implementation
{
    internal class InventoryDataRepository : IDataRepository<IInventory>
    {
        private readonly IDataContext _context;
        private readonly object _inventoryLock = new object();

        public InventoryDataRepository(IDataContext context)
        {
            this._context = context;
        }

        public void Add(IInventory item)
        {
            lock (_inventoryLock)
            {
                _context.Inventories[item.Id] = item;
            }
        }

        public IInventory? Get(Guid id)
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

        public IEnumerable<IInventory> GetAll()
        {
            lock (_inventoryLock)
            {
                return _context.Inventories.Values.ToList();
            }
        }

        public bool Remove(IInventory item)
        {
            lock (_inventoryLock)
            {
                if (_context.Inventories.ContainsKey(item.Id))
                {
                    _context.Inventories.Remove(item.Id);
                    return true;
                }

                return false;
            }
        }

        public bool RemoveById(Guid id)
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

        public bool Update(Guid id, IInventory item)
        {
            lock (_inventoryLock)
            {
                if (_context.Inventories.ContainsKey(id))
                {
                    _context.Inventories[id] = item;
                    return true;
                }

                return false;
            }
        }
    }
}
