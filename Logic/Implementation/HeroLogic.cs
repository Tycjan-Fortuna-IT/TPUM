using Data.API;
using Logic.API;

namespace Logic.Implementation
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
            List<IHeroDataTransferObject> all = new List<IHeroDataTransferObject>();

            foreach (IHero hero in _repository.GetAllHeroes())
            {
                all.Add(Map(hero));
            }

            return all;
        }

        public IHeroDataTransferObject? Get(Guid id)
        {
            IHero? hero = _repository.GetHero(id);

            return hero is not null ? Map(hero) : null;
        }

        public void Add(IHeroDataTransferObject hero)
        {
            _repository.AddHero(new MappedDataHero(hero));
        }

        public bool RemoveById(Guid id)
        {
            return _repository.RemoveHeroById(id);
        }

        public bool Remove(IHeroDataTransferObject hero)
        {
            return _repository.RemoveHero(new MappedDataHero(hero));
        }

        public bool Update(Guid id, IHeroDataTransferObject hero)
        {
            return _repository.UpdateHero(id, new MappedDataHero(hero));
        }

        public void PeriodicItemMaintenanceDeduction()
        {
            // TODO
        }
    }
}
