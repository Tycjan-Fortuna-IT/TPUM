using Data.Implementation;

namespace Data.API
{
    public abstract class DataRepositoryFactory
    {
        public static IDataRepository<IHero> CreateHeroRepository(IDataContext context, IDataRepository<IHero>? dataRepository = default(IDataRepository<IHero>))
        {
            return dataRepository ?? new HeroDataRepository(context);
        }

        public static IDataRepository<IItem> CreateItemRepository(IDataContext context, IDataRepository<IItem>? dataRepository = default(IDataRepository<IItem>))
        {
            return dataRepository ?? new ItemDataRepository(context);
        }

        public static IDataRepository<IInventory> CreateInventoryRepository(IDataContext context, IDataRepository<IInventory>? dataRepository = default(IDataRepository<IInventory>))
        {
            return dataRepository ?? new InventoryDataRepository(context);
        }

        public static IDataRepository<IOrder> CreateOrderRepository(IDataContext context, IDataRepository<IOrder>? dataRepository = default(IDataRepository<IOrder>))
        {
            return dataRepository ?? new OrderDataRepository(context);
        }
    }
}
