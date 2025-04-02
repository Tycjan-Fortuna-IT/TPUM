namespace Presentation.Model.API
{
    public interface IInventoryModelService
    {
        public abstract IEnumerable<IInventoryModel> GetAllInventories();
        public abstract IInventoryModel? GetInventory(Guid id);

        public abstract void Add(Guid id, int capacity);
    }
}