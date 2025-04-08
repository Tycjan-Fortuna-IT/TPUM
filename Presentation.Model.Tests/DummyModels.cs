using Client.Presentation.Model.API;

namespace Client.Presentation.Model.Tests
{
    // Dummy Item Model
    internal class DummyItemModel : IItemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public int MaintenanceCost { get; set; }
    }

    // Dummy Inventory Model
    internal class DummyInventoryModel : IInventoryModel
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public IEnumerable<IItemModel> Items { get; set; } = Enumerable.Empty<IItemModel>();
    }

    // Dummy Hero Model
    internal class DummyHeroModel : IHeroModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Gold { get; set; }
        public IInventoryModel Inventory { get; set; } = default!;
    }

    // Dummy Order Model
    internal class DummyOrderModel : IOrderModel
    {
        public Guid Id { get; set; }
        public IHeroModel Buyer { get; set; } = default!;
        public IEnumerable<IItemModel> ItemsToBuy { get; set; } = Enumerable.Empty<IItemModel>();
    }
}