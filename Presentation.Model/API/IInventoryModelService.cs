namespace Presentation.Model.API
{
    public interface IInventoryModelService
    {
        IEnumerable<IInventoryModel> GetAllInventories();
        IInventoryModel? GetInventory(Guid id);

        void Add(Guid id, int capacity);
    }
}