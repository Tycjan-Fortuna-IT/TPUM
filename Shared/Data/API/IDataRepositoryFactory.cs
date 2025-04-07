namespace ClientServer.Shared.Data.API
{
    public interface IDataRepositoryFactory
    {
        public abstract static IDataRepository CreateDataRepository(IDataContext? context = default, IDataRepository? dataRepository = default);
    }
}
