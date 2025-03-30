using Data.API;
using Logic.API;

namespace Logic.Implementation
{
    internal class OrderLogic : IOrderLogic
    {
        private IDataRepository _repository;

        public OrderLogic(IDataRepository repository)
        {
            this._repository = repository;
        }

        public IEnumerable<IOrderDataTransferObject> GetAll()
        {
            throw new NotImplementedException();
        }

        public IOrderDataTransferObject? Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(IOrderDataTransferObject item)
        {
            throw new NotImplementedException();
        }

        public bool RemoveById(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IOrderDataTransferObject item)
        {
            throw new NotImplementedException();
        }

        public bool Update(Guid id, IOrderDataTransferObject item)
        {
            throw new NotImplementedException();
        }

        public void PeriodicOrderProcessing()
        {
            throw new NotImplementedException();
        }
    }
}
