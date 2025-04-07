using Shared.Data.API;
using Shared.Logic.API;

namespace Client.Logic.Implementation
{
    internal class HeroLogic : IHeroLogic
    {
        private IDataRepository _repository;
        private readonly object _lock = new object();

        public HeroLogic(IDataRepository repository)
        {
            this._repository = repository;
        }

        public static IHeroDataTransferObject Map(IHero hero)
        {
            return new HeroDataTransferObject(hero.Id, hero.Name, hero.Gold, InventoryLogic.Map(hero.Inventory));
        }

        public IEnumerable<IHeroDataTransferObject> GetAll()
        {
            lock (_lock)
            {
                List<IHeroDataTransferObject> all = new List<IHeroDataTransferObject>();

                foreach (IHero hero in _repository.GetAllHeroes())
                {
                    all.Add(Map(hero));
                }

                return all;
            }
        }

        public IHeroDataTransferObject? Get(Guid id)
        {
            lock (_lock)
            {
                IHero? hero = _repository.GetHero(id);

                return hero is not null ? Map(hero) : null;
            }
        }

        public void Add(IHeroDataTransferObject hero)
        {
            lock (_lock)
            {
                _repository.AddHero(new MappedDataHero(hero));
            }
        }

        public bool RemoveById(Guid id)
        {
            lock (_lock)
            {
                return _repository.RemoveHeroById(id);
            }
        }

        public bool Remove(IHeroDataTransferObject hero)
        {
            lock (_lock)
            {
                return _repository.RemoveHero(new MappedDataHero(hero));
            }
        }

        public bool Update(Guid id, IHeroDataTransferObject hero)
        {
            lock (_lock)
            {
                return _repository.UpdateHero(id, new MappedDataHero(hero));
            }
        }

        public void PeriodicItemMaintenanceDeduction()
        {
            lock (_lock)
            {
                foreach (IHeroDataTransferObject hero in GetAll())
                {
                    foreach (IItemDataTransferObject item in hero.Inventory.Items)
                    {
                        hero.Gold -= item.MaintenanceCost;
                    }

                    _repository.UpdateHero(hero.Id, new MappedDataHero(hero));
                }
            }
        }
    }
}
