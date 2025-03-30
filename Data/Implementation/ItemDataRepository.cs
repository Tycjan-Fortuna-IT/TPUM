using Data.API;

namespace Data.Implementation
{
    internal class ItemDataRepository : IDataRepository<IItem>
    {
        private readonly IDataContext _context;
        private readonly object _itemsLock = new object();

        public ItemDataRepository(IDataContext context)
        {
            this._context = context;
        }

        public void Add(IItem item)
        {
            lock (_itemsLock)
            {
                _context.Items[item.Id] = item;
            }
        }

        public IItem? Get(Guid id)
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

        public IEnumerable<IItem> GetAll()
        {
            lock (_itemsLock)
            {
                return _context.Items.Values.ToList();
            }
        }

        public bool Remove(IItem item)
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

        public bool RemoveById(Guid id)
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

        public bool Update(Guid id, IItem item)
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
    }
}
