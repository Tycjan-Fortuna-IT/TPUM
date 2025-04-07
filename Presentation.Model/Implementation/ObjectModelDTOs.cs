using Client.Presentation.Model.API;
using Logic.API;

namespace Client.Presentation.Model.Implementation
{
    // Item DTO
    internal class TransientItemDTO : IItemDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        public TransientItemDTO(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
        public TransientItemDTO(IItemModel model)
            : this(model.Id, model.Name, model.Price, model.MaintenanceCost) { }
    }

    // Inventory DTO
    internal class TransientInventoryDTO : IInventoryDataTransferObject
    {
        public Guid Id { get; }
        public int Capacity { get; }
        public IEnumerable<IItemDataTransferObject> Items { get; }

        public TransientInventoryDTO(Guid id, int capacity, IEnumerable<IItemDataTransferObject> items)
        {
            Id = id;
            Capacity = capacity;
            Items = items?.ToList() ?? new List<IItemDataTransferObject>();
        }
        public TransientInventoryDTO(IInventoryModel model)
           : this(model.Id, model.Capacity, model.Items.Select(i => new TransientItemDTO(i))) { }
    }

    // Hero DTO
    internal class TransientHeroDTO : IHeroDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public float Gold { get; set; }
        public IInventoryDataTransferObject Inventory { get; set; }

        public TransientHeroDTO(Guid id, string name, float gold, IInventoryDataTransferObject inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
        public TransientHeroDTO(IHeroModel model)
             : this(model.Id, model.Name, model.Gold, new TransientInventoryDTO(model.Inventory)) { }
    }

    // Order DTO
    internal class TransientOrderDTO : IOrderDataTransferObject
    {
        public Guid Id { get; }
        public IHeroDataTransferObject Buyer { get; }
        public IEnumerable<IItemDataTransferObject> ItemsToBuy { get; }

        public TransientOrderDTO(Guid id, IHeroDataTransferObject buyer, IEnumerable<IItemDataTransferObject> itemsToBuy)
        {
            Id = id;
            Buyer = buyer;
            ItemsToBuy = itemsToBuy?.ToList() ?? new List<IItemDataTransferObject>();
        }
        public TransientOrderDTO(IOrderModel model)
            : this(model.Id, new TransientHeroDTO(model.Buyer), model.ItemsToBuy.Select(i => new TransientItemDTO(i))) { }
    }
}