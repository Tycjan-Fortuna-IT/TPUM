namespace Client.Logic.API
{
    public interface IConnectionService
    {
        public Task<bool> Connect(Uri peerUri);
        public Task Disconnect();

        public Action? onDataArrived { set; get; }

        public Task FetchItems();
        public Task FetchInventories();
        public Task FetchHeroes();
    }
}
