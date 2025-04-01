using Presentation.Model.API;
using Logic.API;

namespace Presentation.Model.Implementation
{
    internal class HeroModel : IHeroModel
    {
        public Guid Id { get; }
        public string Name { get; }
        public float Gold { get; }
        public IInventoryModel Inventory { get; }

        // DTO
        public HeroModel(IHeroDataTransferObject dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Gold = dto.Gold;
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