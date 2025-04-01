using Logic.API;

namespace Presentation.Model.Implementation.Transient // Internal namespace if desired
{
    // --- Item DTO ---
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
        // Constructor for mapping from IItemModel if needed
        public TransientItemDTO(API.IItemModel model)
            : this(model.Id, model.Name, model.Price, model.MaintenanceCost) { }
    }

    // --- Inventory DTO ---
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
        // Constructor for mapping from IInventoryModel if needed
        public TransientInventoryDTO(API.IInventoryModel model)
           : this(model.Id, model.Capacity, model.Items.Select(i => new TransientItemDTO(i))) { }
    }

    // --- Hero DTO ---
    internal class TransientHeroDTO : IHeroDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; }
        public float Gold { get; }
        public IInventoryDataTransferObject Inventory { get; }

        public TransientHeroDTO(Guid id, string name, float gold, IInventoryDataTransferObject inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
        // Constructor for mapping from IHeroModel if needed
        public TransientHeroDTO(API.IHeroModel model)
             : this(model.Id, model.Name, model.Gold, new TransientInventoryDTO(model.Inventory)) { }
    }

    // --- Order DTO ---
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
        // Constructor for mapping from IOrderModel if needed
        // This needs access to logic or cached models to resolve Buyer/Items from IDs
        // For simplicity, AddOrder in service might construct this differently
        public TransientOrderDTO(API.IOrderModel model)
            : this(model.Id, new TransientHeroDTO(model.Buyer), model.ItemsToBuy.Select(i => new TransientItemDTO(i))) { }
    }
}