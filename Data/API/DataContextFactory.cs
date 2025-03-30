using Data.Implementation;

namespace Data.API
{
    public abstract class DataContextFactory
    {
        public static IDataContext CreateDataContext(IDataContext? dataContext = default(IDataContext))
        {
            return dataContext ?? new DataContext();
        }
    }
}
