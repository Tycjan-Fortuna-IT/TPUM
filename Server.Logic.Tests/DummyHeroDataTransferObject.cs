using ClientServer.Shared.Logic.API;

namespace Server.Logic.Tests
{
    internal class DummyHeroDataTransferObject : IHeroDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public float Gold { get; set; }
        public IInventoryDataTransferObject Inventory { get; set; }

        public DummyHeroDataTransferObject(Guid id, string name, float gold, IInventoryDataTransferObject inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }
}
