// --- Presentation/Model/API/ModelFactoryAbstract.cs ---
using Logic.API; // Needs LogicFactory for default creation signatures

namespace Presentation.Model.API
{
    // Abstract factory for decoupling - Renamed class
    public abstract class ModelFactoryAbstract
    {
        // Method to get the concrete implementation (using default logic)
        // It still returns an instance of the concrete factory, but the method
        // belongs to the abstract class. Consumers call ModelFactoryAbstract.CreateFactory().
        public static ModelFactoryAbstract CreateFactory()
        {
            // Returns the actual implementation (defined in Implementation namespace)
            // The concrete implementation inherits from this abstract class.
            return new Presentation.Model.Implementation.ModelFactory();
        }

        // Methods to create services. Accept optional logic interfaces for injection.
        public abstract IHeroModelService CreateHeroModelService(IHeroLogic? heroLogic = null, IInventoryLogic? inventoryLogic = null);
        public abstract IInventoryModelService CreateInventoryModelService(IInventoryLogic? inventoryLogic = null);
        public abstract IItemModelService CreateItemModelService(IItemLogic? itemLogic = null);
        public abstract IOrderModelService CreateOrderModelService(IOrderLogic? orderLogic = null, IHeroLogic? heroLogic = null, IItemLogic? itemLogic = null);
    }
}