using Client.Data.Websocket;
using Client.Logic.API;
using System.Net.WebSockets;
using System.Text;

namespace Client.Logic.Implementation
{
    public class ClientConnectionService : IConnectionService
    {
        public Action<string> ConnectionLogger { get => _logger; set => _logger = value; }

        public Action? onDataArrived { set; get; }

        private Action<string> _logger = (string message) =>
        {
            Console.WriteLine(message);
        };

        public async Task<bool> Connect(Uri peerUri)
        {
            try
            {
                ConnectionLogger?.Invoke($"Establishing connection to {peerUri.OriginalString}");

                await WebSocketClient.Connect(peerUri, ConnectionLogger!);

                return await Task.FromResult(true);
            }
            catch (WebSocketException e)
            {
                Console.WriteLine($"Caught web socket exception {e.Message}");

                ConnectionLogger?.Invoke(e.Message);

                return await Task.FromResult(false);
            }
        }

        public async Task Disconnect()
        {
            if (WebSocketClient.CurrentConnection == null)
            {
                ConnectionLogger?.Invoke("No connection to server.");
                await Task.FromResult(false);
                return;
            }

            await WebSocketClient.Disconnect();
        }

        public async Task FetchItems()
        {
            if (WebSocketClient.CurrentConnection == null)
            {
                ConnectionLogger?.Invoke("No connection to server.");
                await Task.FromResult(false);
                return;
            }

            await WebSocketClient.CurrentConnection.SendAsync("GET /items");
        }

        public async Task FetchInventories()
        {
            if (WebSocketClient.CurrentConnection == null)
            {
                ConnectionLogger?.Invoke("No connection to server.");
                await Task.FromResult(false);
                return;
            }

            await WebSocketClient.CurrentConnection.SendAsync("GET /inventories");
        }

        public async Task FetchHeroes()
        {
            if (WebSocketClient.CurrentConnection == null)
            {
                ConnectionLogger?.Invoke("No connection to server.");
                await Task.FromResult(false);
                return;
            }

            await WebSocketClient.CurrentConnection.SendAsync("GET /heroes");
        }

        public async Task CreateOrder(Guid id, Guid buyerId, IEnumerable<Guid> itemIds)
        {
            if (WebSocketClient.CurrentConnection == null)
            {
                ConnectionLogger?.Invoke("No connection to server.");
                await Task.FromResult(false);
                return;
            }

            string itemsString = string.Join(",", itemIds);
            string message = $"POST /orders/{id}/buyer/{buyerId}/items/{itemsString}";

            ConnectionLogger?.Invoke($"Creating order: {message}");
            await WebSocketClient.CurrentConnection.SendAsync(message);
        }
    }
}
