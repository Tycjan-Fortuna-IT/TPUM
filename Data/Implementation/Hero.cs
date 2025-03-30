using Data.API;

namespace Data.Implementation
{
    internal class Hero : IHero
    {
        public Guid Id { get; } = Guid.Empty;
        public string Name { get; private set; }
        public float Gold { get; set; }
        public IInventory Inventory { get; private set; }

        public Hero(string name, float gold, IInventory inventory)
        {
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }

        public Hero(Guid id, string name, float gold, IInventory inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }
}
