using Client.Data.Implementation;
using Shared.Data.API;

namespace Client.Data.API
{
    public abstract class DataRepositoryFactory : IDataRepositoryFactory
    {
        public static IDataRepository CreateDataRepository(IDataContext? context = default(IDataContext), IDataRepository? dataRepository = default(IDataRepository))
        {
            return dataRepository ?? new DataRepository(context ?? DataContextFactory.CreateDataContext());
        }
    }
}
