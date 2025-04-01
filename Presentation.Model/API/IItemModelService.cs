namespace Presentation.Model.API
{
    public interface IItemModelService
    {
        IEnumerable<IItemModel> GetAllItems();
        IItemModel? GetItem(Guid id);
        void AddItem(Guid id, string name, int price, int maintenanceCost);
        bool RemoveItem(Guid id);
        bool UpdateItem(Guid id, string name, int price, int maintenanceCost);
    }
}