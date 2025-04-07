namespace Client.Logic.API
{
    public interface IConnectionService
    {
        public Action<string> ConnectionLogger { get; set; }

        public Task<bool> Connect(Uri peerUri);
        public Task Disconnect();
    }
}
