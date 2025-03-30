using Data.API;

namespace Data.Implementation
{
    internal class HeroDataRepository : IDataRepository<IHero>
    {
        private readonly IDataContext _context;
        private readonly object _heroesLock = new object();

        public HeroDataRepository(IDataContext context)
        {
            this._context = context;
        }

        public void Add(IHero item)
        {
            lock (_heroesLock)
            {
                _context.Heroes[item.Id] = item;
            }
        }

        public IHero? Get(Guid id)
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

        public IEnumerable<IHero> GetAll()
        {
            lock (_heroesLock)
            {
                return _context.Heroes.Values.ToList();
            }
        }

        public bool Remove(IHero item)
        {
            lock (_heroesLock)
            {
                if (_context.Heroes.ContainsKey(item.Id))
                {
                    _context.Heroes.Remove(item.Id);
                    return true;
                }

                return false;
            }
        }

        public bool RemoveById(Guid id)
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

        public bool Update(Guid id, IHero item)
        {
            lock (_heroesLock)
            {
                if (_context.Heroes.ContainsKey(id))
                {
                    _context.Heroes[id] = item;
                    return true;
                }

                return false;
            }
        }
    }
}
