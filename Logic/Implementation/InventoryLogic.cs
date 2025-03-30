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
            List<IInventoryDataTransferObject> all = new List<IInventoryDataTransferObject>();

            foreach (IInventory inventory in _repository.GetAllInventories())
            {
                all.Add(Map(inventory));
            }

            return all;
        }

        public IInventoryDataTransferObject? Get(Guid id)
        {
            IInventory? inventory = _repository.GetInventory(id);

            return inventory is not null ? Map(inventory) : null;
        }

        public void Add(IInventoryDataTransferObject inventory)
        {
            _repository.AddInventory(new MappedDataInventory(inventory));
        }

        public bool RemoveById(Guid id)
        {
            return _repository.RemoveInventoryById(id);
        }

        public bool Remove(IInventoryDataTransferObject inventory)
        {
            return _repository.RemoveInventory(new MappedDataInventory(inventory));
        }

        public bool Update(Guid id, IInventoryDataTransferObject inventory)
        {
            return _repository.UpdateInventory(id, new MappedDataInventory(inventory));
        }
    }
}
