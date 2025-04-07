using Shared.Data.API;

namespace Server.Data.Implementation
{
    internal class Item : IItem
    {
        public Guid Id { get; } = Guid.Empty;
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        public Item(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }

        public Item(string name, int price, int maintenanceCost)
        {
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
    }
}
