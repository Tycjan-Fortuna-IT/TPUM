using ClientServer.Shared.Data.API;
using ClientServer.Shared.Logic.API;

namespace Server.Logic.Implementation
{
    internal class MappedDataHero : IHero
    {
        public Guid Id { get; } = Guid.Empty;
        public string Name { get; private set; }
        public float Gold { get; set; }
        public IInventory Inventory { get; private set; }

        public MappedDataHero(IHeroDataTransferObject heroData)
        {
            Id = heroData.Id;
            Name = heroData.Name;
            Gold = heroData.Gold;
            Inventory = new MappedDataInventory(heroData.Inventory);
        }
    }
}
