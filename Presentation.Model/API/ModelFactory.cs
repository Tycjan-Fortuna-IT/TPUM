using Logic.API;
using Presentation.Model.Implementation;

namespace Presentation.Model.API
{
    public abstract class ModelFactory
    {
        // Helper stays the same or can be inlined
        private static TLogic ResolveLogic<TLogic>(TLogic? injectedLogic, Func<TLogic> defaultStaticCreator) where TLogic : class
        {
            return injectedLogic ?? defaultStaticCreator();
        }

        // Static creation methods directly
        public static IHeroModelService CreateHeroModelService(IHeroLogic? heroLogic = null, IInventoryLogic? inventoryLogic = null)
        {
            var resolvedHeroLogic = ResolveLogic(heroLogic, () => LogicFactory.CreateHeroLogic());
            var resolvedInventoryLogic = ResolveLogic(inventoryLogic, () => LogicFactory.CreateInventoryLogic());
            return new HeroModelService(resolvedHeroLogic, resolvedInventoryLogic);
        }

        public static IInventoryModelService CreateInventoryModelService(IInventoryLogic? inventoryLogic = null)
        {
            var resolvedLogic = ResolveLogic(inventoryLogic, () => LogicFactory.CreateInventoryLogic());
            return new InventoryModelService(resolvedLogic);
        }

        public static IItemModelService CreateItemModelService(IItemLogic? itemLogic = null)
        {
            var resolvedLogic = ResolveLogic(itemLogic, () => LogicFactory.CreateItemLogic());
            return new ItemModelService(resolvedLogic);
        }

        public static IOrderModelService CreateOrderModelService(IOrderLogic? orderLogic = null, IHeroLogic? heroLogic = null, IItemLogic? itemLogic = null)
        {
            var resolvedOrderLogic = ResolveLogic(orderLogic, () => LogicFactory.CreateOrderLogic());
            var resolvedHeroLogic = ResolveLogic(heroLogic, () => LogicFactory.CreateHeroLogic());
            var resolvedItemLogic = ResolveLogic(itemLogic, () => LogicFactory.CreateItemLogic());
            return new OrderModelService(resolvedOrderLogic, resolvedHeroLogic, resolvedItemLogic);
        }
    }
}