using Data.Implementation;

namespace Data.API
{
    public abstract class DataRepositoryFactory
    {
        public static IDataRepository CreateDataRepository(IDataContext? context = default(IDataContext), IDataRepository? dataRepository = default(IDataRepository))
        {
            return dataRepository ?? new DataRepository(context ?? DataContextFactory.CreateDataContext());
        }
    }
}
