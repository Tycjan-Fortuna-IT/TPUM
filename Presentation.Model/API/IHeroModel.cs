
namespace Presentation.Model.API
{
    public interface IHeroModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Gold { get; set; }
        public IInventoryModel Inventory { get; set; }
    }
}
