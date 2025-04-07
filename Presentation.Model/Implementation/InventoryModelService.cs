using Client.Presentation.Model.API;
using Shared.Logic.API;

namespace Client.Presentation.Model.Implementation
{
    internal class InventoryModelService : IInventoryModelService
    {
        private readonly IInventoryLogic _inventoryLogic;

        public InventoryModelService(IInventoryLogic inventoryLogic)
        {
            _inventoryLogic = inventoryLogic ?? throw new ArgumentNullException(nameof(inventoryLogic));
        }

        public void Add(Guid id, int capacity)
        {
            _inventoryLogic.Add(new TransientInventoryDTO(id, capacity, new List<IItemDataTransferObject>()));
        }

        public IEnumerable<IInventoryModel> GetAllInventories()
        {
            return _inventoryLogic.GetAll()
                                 .Select(dto => new InventoryModel(dto)); // Map DTO to Model
        }

        public IInventoryModel? GetInventory(Guid id)
        {
            IInventoryDataTransferObject? dto = _inventoryLogic.Get(id);
            return dto == null ? null : new InventoryModel(dto); // Map DTO to Model
        }
    }
}