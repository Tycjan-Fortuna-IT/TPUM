using Logic.API;

namespace Logic.Tests
{
    internal class DummyHeroDataTransferObject : IHeroDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; }
        public float Gold { get; }
        public IInventoryDataTransferObject Inventory { get; }

        public DummyHeroDataTransferObject(Guid id, string name, float gold, IInventoryDataTransferObject inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }
}
