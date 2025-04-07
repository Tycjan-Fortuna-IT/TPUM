namespace Client.Logic.API
{
    public interface IConnectionService
    {
        public Task<bool> Connect(Uri peerUri);
        public Task Disconnect();

        public Task FetchItems();
    }
}
