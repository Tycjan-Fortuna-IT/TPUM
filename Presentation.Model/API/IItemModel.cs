using Presentation.Model.Implementation;

namespace Presentation.Model.API
{
    public interface IItemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int MaintenanceCost { get; set; }
    }
}
