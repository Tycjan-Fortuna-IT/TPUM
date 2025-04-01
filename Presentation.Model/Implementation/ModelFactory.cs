using Presentation.Model.API;
using Logic.API;
using System;

namespace Presentation.Model.Implementation
{
    internal class ModelFactory : ModelFactoryAbstract
    {
        public ModelFactory() { }

        private TLogic ResolveLogic<TLogic>(TLogic? injectedLogic, Func<TLogic> defaultStaticCreator) where TLogic : class
        {
            if (injectedLogic != null)
            {
                return injectedLogic;
            }
            return defaultStaticCreator();
        }

        // wrap methods to make this layer independent
        public override IHeroModelService CreateHeroModelService(IHeroLogic? heroLogic = null, IInventoryLogic? inventoryLogic = null)
        {
            var resolvedHeroLogic = ResolveLogic(heroLogic, () => LogicFactory.CreateHeroLogic());
            var resolvedInventoryLogic = ResolveLogic(inventoryLogic, () => LogicFactory.CreateInventoryLogic());

            return new HeroModelService(resolvedHeroLogic, resolvedInventoryLogic);
        }

        public override IInventoryModelService CreateInventoryModelService(IInventoryLogic? inventoryLogic = null)
        {
            var resolvedLogic = ResolveLogic(inventoryLogic, () => LogicFactory.CreateInventoryLogic());
            return new InventoryModelService(resolvedLogic);
        }

        public override IItemModelService CreateItemModelService(IItemLogic? itemLogic = null)
        {
            var resolvedLogic = ResolveLogic(itemLogic, () => LogicFactory.CreateItemLogic());
            return new ItemModelService(resolvedLogic);
        }

        public override IOrderModelService CreateOrderModelService(IOrderLogic? orderLogic = null, IHeroLogic? heroLogic = null, IItemLogic? itemLogic = null)
        {
            var resolvedOrderLogic = ResolveLogic(orderLogic, () => LogicFactory.CreateOrderLogic());
            var resolvedHeroLogic = ResolveLogic(heroLogic, () => LogicFactory.CreateHeroLogic());
            var resolvedItemLogic = ResolveLogic(itemLogic, () => LogicFactory.CreateItemLogic());

            return new OrderModelService(resolvedOrderLogic, resolvedHeroLogic, resolvedItemLogic);
        }
    }
}