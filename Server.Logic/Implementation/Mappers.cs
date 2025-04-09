using ClientServer.Shared.Data.API;
using ClientServer.Shared.Logic.API;

namespace Server.Logic.Implementation
{
    internal class MappedDataHero : IHero
    {
        public Guid Id { get; } = Guid.Empty;
        public string Name { get; private set; }
        public float Gold { get; set; }
        public IInventory Inventory { get; private set; }

        public MappedDataHero(IHeroDataTransferObject heroData)
        {
            Id = heroData.Id;
            Name = heroData.Name;
            Gold = heroData.Gold;
            Inventory = new MappedDataInventory(heroData.Inventory);
        }
    }

    internal class MappedDataInventory : IInventory
    {
        public Guid Id { get; } = Guid.Empty;
        public int Capacity { get; }
        public List<IItem> Items { get; }

        public MappedDataInventory(IInventoryDataTransferObject inventoryData)
        {
            List<IItem> mappedItems = new List<IItem>();

            foreach (IItemDataTransferObject item in inventoryData.Items)
            {
                mappedItems.Add(new MappedDataItem(item));
            }

            Id = inventoryData.Id;
            Capacity = inventoryData.Capacity;
            Items = mappedItems;
        }
    }

    internal class MappedDataItem : IItem
    {
        public Guid Id { get; } = Guid.Empty;
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        public MappedDataItem(IItemDataTransferObject itemData)
        {
            Id = itemData.Id;
            Name = itemData.Name;
            Price = itemData.Price;
            MaintenanceCost = itemData.MaintenanceCost;
        }
    }

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
