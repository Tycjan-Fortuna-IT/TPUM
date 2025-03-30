using Presentation.Model.API;

namespace Presentation.Model.Implementation
{
    public class InventoryModel : IInventoryModel
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public List<IItemModel> Items { get; set; }
    }
}
