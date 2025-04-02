namespace Presentation.Model.API
{
    public interface IItemModelService
    {
        public abstract IEnumerable<IItemModel> GetAllItems();
        public abstract IItemModel? GetItem(Guid id);
        public abstract void AddItem(Guid id, string name, int price, int maintenanceCost);
        public abstract bool RemoveItem(Guid id);
        public abstract bool UpdateItem(Guid id, string name, int price, int maintenanceCost);
    }
}