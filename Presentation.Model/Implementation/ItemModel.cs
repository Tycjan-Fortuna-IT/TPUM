using Presentation.Model.API;

namespace Presentation.Model.Implementation
{
    public class ItemModel : IItemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int MaintenanceCost { get; set; }
    }
}
