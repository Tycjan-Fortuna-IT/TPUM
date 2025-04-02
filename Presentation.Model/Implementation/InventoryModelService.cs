using Presentation.Model.API;
using Logic.API;
using System;
using System.Collections.Generic;

namespace Presentation.Model.Implementation
{
    public class InventoryModelService : IInventoryModelService
    {
        private readonly IInventoryLogic _inventoryLogic;

        public InventoryModelService(IInventoryLogic inventoryLogic)
        {
            _inventoryLogic = inventoryLogic ?? throw new ArgumentNullException(nameof(inventoryLogic));
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