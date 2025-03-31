
namespace Presentation.Model.API
{
    public interface IInventoryModel
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public List<IItemModel> Items { get; set; }
    }
}
