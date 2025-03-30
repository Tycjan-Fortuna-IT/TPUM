
namespace Presentation.Model.API
{
    public interface IOrderModel
    {
        public Guid Id { get; set; }
        public IHeroModel Buyer { get; set; }
        public List<IItemModel> ItemsToBuy { get; set; }
    }
}
