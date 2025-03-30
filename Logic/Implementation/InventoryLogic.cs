using Data.API;
using Logic.API;

namespace Logic.Implementation
{
    internal class InventoryLogic : IInventoryLogic
    {
        private IDataRepository _repository;

        public InventoryLogic(IDataRepository repository)
        {
            this._repository = repository;
        }

        public IEnumerable<IInventoryDataTransferObject> GetAll()
        {
            throw new NotImplementedException();
        }

        public IInventoryDataTransferObject? Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(IInventoryDataTransferObject item)
        {
            throw new NotImplementedException();
        }

        public bool RemoveById(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IInventoryDataTransferObject item)
        {
            throw new NotImplementedException();
        }

        public bool Update(Guid id, IInventoryDataTransferObject item)
        {
            throw new NotImplementedException();
        }
    }
}
