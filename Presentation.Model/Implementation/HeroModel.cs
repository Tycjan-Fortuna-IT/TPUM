using Presentation.Model.API;

namespace Presentation.Model.Implementation
{
    internal class HeroModel : IHeroModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Gold { get; set; }
        public IInventoryModel Inventory { get; set; }
    }
}
