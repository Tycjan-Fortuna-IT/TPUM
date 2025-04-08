using ClientServer.Shared.Data.API;

namespace Server.Data.Tests
{
    internal class DummyItem : IItem
    {
        public Guid Id { get; } = Guid.Empty;
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        public DummyItem(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }

        public DummyItem(string name, int price, int maintenanceCost)
        {
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
    }
}
