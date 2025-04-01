using System.Collections.Generic;

namespace Presentation.Model.API
{
    public interface IInventoryModelService
    {
        IEnumerable<IInventoryModel> GetAllInventories();
        IInventoryModel? GetInventory(Guid id);
    }
}