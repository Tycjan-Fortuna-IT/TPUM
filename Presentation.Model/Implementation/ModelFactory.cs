// --- Presentation/Model/Implementation/ModelFactory.cs ---
using Presentation.Model.API; // Needs API interfaces
using Logic.API; // Needs Logic interfaces and the static LogicFactory
using System;

namespace Presentation.Model.Implementation
{
    // Concrete Factory Implementation
    internal class ModelFactory : API.ModelFactoryAbstract // Inherit from the renamed abstract API factory
    {
        // Default constructor is sufficient
        public ModelFactory() { }

        // Helper to resolve logic: Use injected instance if provided, otherwise use static factory method.
        // defaultStaticCreator is a delegate that takes NO arguments and returns TLogic.
        private TLogic ResolveLogic<TLogic>(TLogic? injectedLogic, Func<TLogic> defaultStaticCreator) where TLogic : class
        {
            if (injectedLogic != null)
            {
                return injectedLogic;
            }
            // Invoke the delegate to create the default instance
            return defaultStaticCreator();
        }

        public override IHeroModelService CreateHeroModelService(IHeroLogic? heroLogic = null, IInventoryLogic? inventoryLogic = null)
        {
            // Wrap the static factory calls in parameterless lambdas () => ...
            var resolvedHeroLogic = ResolveLogic(heroLogic, () => Logic.API.LogicFactory.CreateHeroLogic()); // FIXED
            var resolvedInventoryLogic = ResolveLogic(inventoryLogic, () => Logic.API.LogicFactory.CreateInventoryLogic()); // FIXED

            return new HeroModelService(resolvedHeroLogic, resolvedInventoryLogic);
        }

        public override IInventoryModelService CreateInventoryModelService(IInventoryLogic? inventoryLogic = null)
        {
            // Wrap the static factory call in a parameterless lambda
            var resolvedLogic = ResolveLogic(inventoryLogic, () => Logic.API.LogicFactory.CreateInventoryLogic()); // FIXED
            return new InventoryModelService(resolvedLogic);
        }

        public override IItemModelService CreateItemModelService(IItemLogic? itemLogic = null)
        {
            // Wrap the static factory call in a parameterless lambda
            var resolvedLogic = ResolveLogic(itemLogic, () => Logic.API.LogicFactory.CreateItemLogic()); // FIXED
            return new ItemModelService(resolvedLogic);
        }

        public override IOrderModelService CreateOrderModelService(IOrderLogic? orderLogic = null, IHeroLogic? heroLogic = null, IItemLogic? itemLogic = null)
        {
            // Wrap the static factory calls in parameterless lambdas
            var resolvedOrderLogic = ResolveLogic(orderLogic, () => Logic.API.LogicFactory.CreateOrderLogic()); // FIXED
            var resolvedHeroLogic = ResolveLogic(heroLogic, () => Logic.API.LogicFactory.CreateHeroLogic()); // FIXED
            var resolvedItemLogic = ResolveLogic(itemLogic, () => Logic.API.LogicFactory.CreateItemLogic()); // FIXED

            return new OrderModelService(resolvedOrderLogic, resolvedHeroLogic, resolvedItemLogic);
        }
    }
}