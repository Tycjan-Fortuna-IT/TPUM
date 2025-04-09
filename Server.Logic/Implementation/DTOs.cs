using ClientServer.Shared.Logic.API;

namespace Server.Logic.Implementation
{
    internal class HeroDataTransferObject : IHeroDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public float Gold { get; set; }
        public IInventoryDataTransferObject Inventory { get; set; }

        public HeroDataTransferObject(Guid id, string name, float gold, IInventoryDataTransferObject inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }

    internal class InventoryDataTransferObject : IInventoryDataTransferObject
    {
        public Guid Id { get; }
        public int Capacity { get; }
        public IEnumerable<IItemDataTransferObject> Items { get; }

        public InventoryDataTransferObject(Guid id, int capacity, IEnumerable<IItemDataTransferObject> items)
        {
            Id = id;
            Capacity = capacity;
            Items = items;
        }
    }

    internal class ItemDataTransferObject : IItemDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        public ItemDataTransferObject(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
    }

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
