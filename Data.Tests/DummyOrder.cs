using Data.API;

namespace Data.Tests
{
    internal class DummyOrder : IOrder
    {
        public Guid Id { get; } = Guid.Empty;
        public IHero Buyer { get; }
        public IEnumerable<IItem> ItemsToBuy { get; }

        public DummyOrder(IHero buyer, IEnumerable<IItem> itemsToBuy)
        {
            Buyer = buyer;
            ItemsToBuy = itemsToBuy;
        }

        public DummyOrder(Guid id, IHero buyer, IEnumerable<IItem> itemsToBuy)
        {
            Id = id;
            Buyer = buyer;
            ItemsToBuy = itemsToBuy;
        }
    }
}
