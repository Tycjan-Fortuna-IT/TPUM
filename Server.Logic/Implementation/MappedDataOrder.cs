using ClientServer.Shared.Data.API;
using ClientServer.Shared.Logic.API;

namespace Server.Logic.Implementation
{
    internal class MappedDataOrder : IOrder
    {
        public Guid Id { get; } = Guid.Empty;
        public IHero Buyer { get; }
        public IEnumerable<IItem> ItemsToBuy { get; }

        public MappedDataOrder(IOrderDataTransferObject orderData)
        {
            List<IItem> mappedItems = new List<IItem>();

            foreach (IItemDataTransferObject item in orderData.ItemsToBuy)
            {
                mappedItems.Add(new MappedDataItem(item));
            }

            Id = orderData.Id;
            Buyer = new MappedDataHero(orderData.Buyer);
            ItemsToBuy = mappedItems;
        }
    }
}
