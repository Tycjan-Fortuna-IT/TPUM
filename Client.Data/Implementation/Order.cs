using Shared.Data.API;

namespace Client.Data.Implementation
{
    internal class Order : IOrder
    {
        public Guid Id { get; } = Guid.Empty;
        public IHero Buyer { get; }
        public IEnumerable<IItem> ItemsToBuy { get; }

        public Order(IHero buyer, IEnumerable<IItem> itemsToBuy)
        {
            Buyer = buyer;
            ItemsToBuy = itemsToBuy;
        }

        public Order(Guid id, IHero buyer, IEnumerable<IItem> itemsToBuy)
        {
            Id = id;
            Buyer = buyer;
            ItemsToBuy = itemsToBuy;
        }
    }
}
