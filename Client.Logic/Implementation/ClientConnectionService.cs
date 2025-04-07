using Client.Data.Websocket;
using Client.Logic.API;
using System.Net.WebSockets;

namespace Client.Logic.Implementation
{
    public class ClientConnectionService : IConnectionService
    {
        public Action<string> ConnectionLogger { get => _logger; set => _logger = value; }

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
            await WebSocketClient.Disconnect();
        }

        public async Task FetchItems()
        {
            if (WebSocketClient.CurrentConnection != null)
            {
                await WebSocketClient.CurrentConnection.SendAsync("GET /items");
            }
            else
            {
                ConnectionLogger?.Invoke("No connection to server.");
            }
        }
    }
}
