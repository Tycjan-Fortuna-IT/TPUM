using ClientServer.Shared.Logic.API;

namespace Server.Logic.Tests
{
    internal class DummyItemDataTransferObject : IItemDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        public DummyItemDataTransferObject(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
    }
}
