using Data.API;

namespace Data.Implementation
{
    internal class OrderDataRepository : IDataRepository<IOrder>
    {
        private readonly IDataContext _context;
        private readonly object _ordersLock = new object();

        public OrderDataRepository(IDataContext context)
        {
            this._context = context;
        }

        public void Add(IOrder item)
        {
            lock (_ordersLock)
            {
                _context.Orders[item.Buyer.Id] = item;
            }
        }

        public IOrder? Get(Guid id)
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

        public IEnumerable<IOrder> GetAll()
        {
            lock (_ordersLock)
            {
                return _context.Orders.Values.ToList();
            }
        }

        public bool Remove(IOrder order)
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

        public bool RemoveById(Guid id)
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

        public bool Update(Guid id, IOrder item)
        {
            lock (_ordersLock)
            {
                if (_context.Orders.ContainsKey(id))
                {
                    _context.Orders[id] = item;
                    return true;
                }

                return false;
            }
        }
    }
}
