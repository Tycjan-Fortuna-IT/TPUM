using Shared.Data.API;

namespace Shared.Logic.API
{
    public interface ILogicFactory
    {
        public abstract static IHeroLogic CreateHeroLogic(IDataRepository? dataRepository = default(IDataRepository));

        public abstract static IInventoryLogic CreateInventoryLogic(IDataRepository? dataRepository = default(IDataRepository));

        public abstract static IItemLogic CreateItemLogic(IDataRepository? dataRepository = default(IDataRepository));

        public abstract static IOrderLogic CreateOrderLogic(IDataRepository? dataRepository = default(IDataRepository));
    }
}
