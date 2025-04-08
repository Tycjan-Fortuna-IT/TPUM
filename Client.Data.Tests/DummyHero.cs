using ClientServer.Shared.Data.API;

namespace Client.Data.Tests
{
    internal class DummyHero : IHero
    {
        public Guid Id { get; } = Guid.Empty;
        public string Name { get; private set; }
        public float Gold { get; set; }
        public IInventory Inventory { get; private set; }

        public DummyHero(string name, float gold, IInventory inventory)
        {
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }

        public DummyHero(Guid id, string name, float gold, IInventory inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }
}
