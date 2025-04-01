using Presentation.Model.API;

namespace Presentation.Model.Implementation
{
    internal class HeroModel : IHeroModel
    {
        public Guid Id { get; }
        public string Name { get; }
        public float Gold { get; }
        public IInventoryModel Inventory { get; }

        // Constructor taking Logic DTO for mapping
        public HeroModel(Logic.API.IHeroDataTransferObject dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Gold = dto.Gold;
            //Inventory DTO -> InventoryModel
            Inventory = new InventoryModel(dto.Inventory);
        }

        // direct creation
        public HeroModel(Guid id, string name, float gold, IInventoryModel inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }
}