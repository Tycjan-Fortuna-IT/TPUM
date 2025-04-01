using Logic.API;

namespace Presentation.Model.API
{
    public abstract class ModelFactoryAbstract
    {
        public static ModelFactoryAbstract CreateFactory()
        {
            return new Implementation.ModelFactory();
        }

        public abstract IHeroModelService CreateHeroModelService(IHeroLogic? heroLogic = null, IInventoryLogic? inventoryLogic = null);
        public abstract IInventoryModelService CreateInventoryModelService(IInventoryLogic? inventoryLogic = null);
        public abstract IItemModelService CreateItemModelService(IItemLogic? itemLogic = null);
        public abstract IOrderModelService CreateOrderModelService(IOrderLogic? orderLogic = null, IHeroLogic? heroLogic = null, IItemLogic? itemLogic = null);
    }
}