using Shared.Data.API;
using Shared.Logic.API;

namespace Client.Logic.Implementation
{
    internal class InventoryLogic : IInventoryLogic
    {
        private IDataRepository _repository;
        private readonly object _lock = new object();

        public InventoryLogic(IDataRepository repository)
        {
            this._repository = repository;
        }

        public static IInventoryDataTransferObject Map(IInventory inventory)
        {
            List<IItemDataTransferObject> mappedItems = new List<IItemDataTransferObject>();

            foreach (IItem item in inventory.Items)
            {
                mappedItems.Add(ItemLogic.Map(item));
            }

            return new InventoryDataTransferObject(inventory.Id, inventory.Capacity, mappedItems);
        }

        public IEnumerable<IInventoryDataTransferObject> GetAll()
        {
            lock (_lock)
            {
                List<IInventoryDataTransferObject> all = new List<IInventoryDataTransferObject>();

                foreach (IInventory inventory in _repository.GetAllInventories())
                {
                    all.Add(Map(inventory));
                }

                return all;
            }
        }

        public IInventoryDataTransferObject? Get(Guid id)
        {
            lock (_lock)
            {
                IInventory? inventory = _repository.GetInventory(id);

                return inventory is not null ? Map(inventory) : null;
            }
        }

        public void Add(IInventoryDataTransferObject inventory)
        {
            lock (_lock)
            {
                _repository.AddInventory(new MappedDataInventory(inventory));
            }
        }

        public bool RemoveById(Guid id)
        {
            lock (_lock)
            {
                return _repository.RemoveInventoryById(id);
            }
        }

        public bool Remove(IInventoryDataTransferObject inventory)
        {
            lock (_lock)
            {
                return _repository.RemoveInventory(new MappedDataInventory(inventory));
            }
        }

        public bool Update(Guid id, IInventoryDataTransferObject inventory)
        {
            lock (_lock)
            {
                return _repository.UpdateInventory(id, new MappedDataInventory(inventory));
            }
        }
    }
}
