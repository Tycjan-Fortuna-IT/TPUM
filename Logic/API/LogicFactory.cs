using Data.API;
using Logic.Implementation;

namespace Logic.API
{
    public abstract class LogicFactory
    {
        public static IHeroLogic CreateHeroLogic(IDataRepository? dataRepository = default(IDataRepository))
        {
            return new HeroLogic(dataRepository ?? DataRepositoryFactory.CreateDataRepository());
        }

        public static IInventoryLogic CreateInventoryLogic(IDataRepository? dataRepository = default(IDataRepository))
        {
            return new InventoryLogic(dataRepository ?? DataRepositoryFactory.CreateDataRepository());
        }

        public static IItemLogic CreateItemLogic(IDataRepository? dataRepository = default(IDataRepository))
        {
            return new ItemLogic(dataRepository ?? DataRepositoryFactory.CreateDataRepository());
        }

        public static IOrderLogic CreateOrderLogic(IDataRepository? dataRepository = default(IDataRepository))
        {
            return new OrderLogic(dataRepository ?? DataRepositoryFactory.CreateDataRepository());
        }
    }
}
