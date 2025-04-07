using Server.Data.Implementation;
using ClientServer.Shared.Data.API;

namespace Server.Data.API
{
    public abstract class DataRepositoryFactory : IDataRepositoryFactory
    {
        public static IDataRepository CreateDataRepository(IDataContext? context = default(IDataContext), IDataRepository? dataRepository = default(IDataRepository))
        {
            return dataRepository ?? new DataRepository(context ?? DataContextFactory.CreateDataContext());
        }
    }
}
