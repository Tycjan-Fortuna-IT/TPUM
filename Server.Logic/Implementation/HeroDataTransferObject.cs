using Shared.Logic.API;

namespace Server.Logic.Implementation
{
    internal class HeroDataTransferObject : IHeroDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public float Gold { get; set; }
        public IInventoryDataTransferObject Inventory { get; set; }

        public HeroDataTransferObject(Guid id, string name, float gold, IInventoryDataTransferObject inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }
}
