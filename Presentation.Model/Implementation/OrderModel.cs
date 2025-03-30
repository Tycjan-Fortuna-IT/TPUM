using Presentation.Model.API;

namespace Presentation.Model.Implementation
{
    internal class OrderModel : IOrderModel
    {
        public Guid Id { get; set; }
        public IHeroModel Buyer { get; set; }
        public List<IItemModel> ItemsToBuy { get; set; }
    }
}
