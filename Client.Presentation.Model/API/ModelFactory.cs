using Client.Logic.API;
using Client.Presentation.Model.Implementation;
using ClientServer.Shared.Logic.API;

namespace Client.Presentation.Model.API
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
            IHeroLogic resolvedHeroLogic = ResolveLogic(heroLogic, () => LogicFactory.CreateHeroLogic());
            IInventoryLogic resolvedInventoryLogic = ResolveLogic(inventoryLogic, () => LogicFactory.CreateInventoryLogic());
            return new HeroModelService(resolvedHeroLogic, resolvedInventoryLogic);
        }

        public static IInventoryModelService CreateInventoryModelService(IInventoryLogic? inventoryLogic = null)
        {
            IInventoryLogic resolvedLogic = ResolveLogic(inventoryLogic, () => LogicFactory.CreateInventoryLogic());
            return new InventoryModelService(resolvedLogic);
        }

        public static IItemModelService CreateItemModelService(IItemLogic? itemLogic = null)
        {
            IItemLogic resolvedLogic = ResolveLogic(itemLogic, () => LogicFactory.CreateItemLogic());
            return new ItemModelService(resolvedLogic);
        }

        public static IOrderModelService CreateOrderModelService(IOrderLogic? orderLogic = null, IHeroLogic? heroLogic = null, IItemLogic? itemLogic = null)
        {
            IOrderLogic resolvedOrderLogic = ResolveLogic(orderLogic, () => LogicFactory.CreateOrderLogic());
            IHeroLogic resolvedHeroLogic = ResolveLogic(heroLogic, () => LogicFactory.CreateHeroLogic());
            IItemLogic resolvedItemLogic = ResolveLogic(itemLogic, () => LogicFactory.CreateItemLogic());
            return new OrderModelService(resolvedOrderLogic, resolvedHeroLogic, resolvedItemLogic);
        }
    }
}