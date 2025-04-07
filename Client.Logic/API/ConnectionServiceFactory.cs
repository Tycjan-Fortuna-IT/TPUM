using Client.Logic.Implementation;

namespace Client.Logic.API
{
    public abstract class ConnectionServiceFactory
    {
        public static IConnectionService CreateConnectionService(IConnectionService? service = default(IConnectionService))
        {
            return service ?? new ClientConnectionService();
        }
    }
}
