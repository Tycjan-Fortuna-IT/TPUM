
namespace Presentation.Model.API
{
    public interface IOrderModel
    {
        public Guid Id { get; set; }
        public IHeroModel Buyer { get; set; }
        public List<IItemModel> ItemsToBuy { get; set; }

        public static IOrderModel CreateOrder(IHeroModel buyer, List<IItemModel> itemsToBuy)
        {
            var order = new Implementation.OrderModel
            {
                Id = Guid.NewGuid(),
                Buyer = buyer,
                ItemsToBuy = itemsToBuy
            };

            return order;
        }

    }
}
