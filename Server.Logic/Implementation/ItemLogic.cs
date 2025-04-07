using Shared.Data.API;
using Shared.Logic.API;

namespace Server.Logic.Implementation
{
    internal class ItemLogic : IItemLogic
    {
        private IDataRepository _repository;
        private readonly object _lock = new object();

        public ItemLogic(IDataRepository repository)
        {
            this._repository = repository;
        }

        public static IItemDataTransferObject Map(IItem item)
        {
            return new ItemDataTransferObject(item.Id, item.Name, item.Price, item.MaintenanceCost);
        }

        public IEnumerable<IItemDataTransferObject> GetAll()
        {
            lock (_lock)
            {
                List<IItemDataTransferObject> all = new List<IItemDataTransferObject>();

                foreach (IItem item in _repository.GetAllItems())
                {
                    all.Add(Map(item));
                }

                return all;
            }
        }

        public IItemDataTransferObject? Get(Guid id)
        {
            lock (_lock)
            {
                IItem? item = _repository.GetItem(id);

                return item is not null ? Map(item) : null;
            }
        }

        public void Add(IItemDataTransferObject item)
        {
            lock (_lock)
            {
                _repository.AddItem(new MappedDataItem(item));
            }
        }

        public bool RemoveById(Guid id)
        {
            lock (_lock)
            {
                return _repository.RemoveItemById(id);
            }
        }

        public bool Remove(IItemDataTransferObject item)
        {
            lock (_lock)
            {
                return _repository.RemoveItem(new MappedDataItem(item));
            }
        }

        public bool Update(Guid id, IItemDataTransferObject item)
        {
            lock (_lock)
            {
                return _repository.UpdateItem(id, new MappedDataItem(item));
            }
        }
    }
}
