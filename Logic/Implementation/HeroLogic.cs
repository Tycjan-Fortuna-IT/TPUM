using Data.API;
using Logic.API;

namespace Logic.Implementation
{
    internal class HeroLogic : IHeroLogic
    {
        private IDataRepository _repository;

        public HeroLogic(IDataRepository repository)
        {
            this._repository = repository;
        }

        public IEnumerable<IHeroDataTransferObject> GetAll()
        {
            throw new NotImplementedException();
        }

        public IHeroDataTransferObject? Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(IHeroDataTransferObject item)
        {
            throw new NotImplementedException();
        }

        public bool RemoveById(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IHeroDataTransferObject item)
        {
            throw new NotImplementedException();
        }

        public bool Update(Guid id, IHeroDataTransferObject item)
        {
            throw new NotImplementedException();
        }

        public void PeriodicItemMaintenanceDeduction()
        {
            throw new NotImplementedException();
        }
    }
}
