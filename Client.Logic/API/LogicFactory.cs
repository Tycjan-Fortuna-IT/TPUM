using Client.Data.API;
using Client.Logic.Implementation;
using Shared.Data.API;
using Shared.Logic.API;

namespace Client.Logic.API
{
    public abstract class LogicFactory : ILogicFactory
    {
        public static IHeroLogic CreateHeroLogic(IDataRepository? dataRepository = default(IDataRepository))
        {
            return new HeroLogic(dataRepository ?? _repository);
        }

        public static IInventoryLogic CreateInventoryLogic(IDataRepository? dataRepository = default(IDataRepository))
        {
            return new InventoryLogic(dataRepository ?? _repository);
        }

        public static IItemLogic CreateItemLogic(IDataRepository? dataRepository = default(IDataRepository))
        {
            return new ItemLogic(dataRepository ?? _repository);
        }

        public static IOrderLogic CreateOrderLogic(IDataRepository? dataRepository = default(IDataRepository))
        {
            return new OrderLogic(dataRepository ?? _repository);
        }

        private static IDataRepository _repository = DataRepositoryFactory.CreateDataRepository();
    }
}
