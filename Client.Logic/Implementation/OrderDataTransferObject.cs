using Shared.Logic.API;

namespace Client.Logic.Implementation
{
    internal class OrderDataTransferObject : IOrderDataTransferObject
    {
        public Guid Id { get; }
        public IHeroDataTransferObject Buyer { get; }
        public IEnumerable<IItemDataTransferObject> ItemsToBuy { get; }

        public OrderDataTransferObject(Guid id, IHeroDataTransferObject buyer, IEnumerable<IItemDataTransferObject> itemsToBuy)
        {
            Id = id;
            Buyer = buyer;
            ItemsToBuy = itemsToBuy;
        }
    }
}
