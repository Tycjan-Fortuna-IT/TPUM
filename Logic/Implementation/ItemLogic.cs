using Data.API;
using Logic.API;

namespace Logic.Implementation
{
    internal class ItemLogic : IItemLogic
    {
        private IDataRepository _repository;

        public ItemLogic(IDataRepository repository)
        {
            this._repository = repository;
        }

        public IEnumerable<IItemDataTransferObject> GetAll()
        {
            throw new NotImplementedException();
        }

        public IItemDataTransferObject? Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Add(IItemDataTransferObject item)
        {
            throw new NotImplementedException();
        }

        public bool RemoveById(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IItemDataTransferObject item)
        {
            throw new NotImplementedException();
        }

        public bool Update(Guid id, IItemDataTransferObject item)
        {
            throw new NotImplementedException();
        }
    }
}
