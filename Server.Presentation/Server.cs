using Shared.Data.API;
using System.Globalization;

namespace Server.Presentation
{
    internal static class Server
    {
        private static WebSocketConnection CurrentConnection = null!;

        private static async Task Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            Console.WriteLine("[Server]: Starting WebSocket server on port 8081...");
            await WebSocketServer.Server(8081, ConnectionHandler);
        }

        private static void ConnectionHandler(WebSocketConnection webSocketConnection)
        {
            CurrentConnection = webSocketConnection;
            webSocketConnection.onMessage = ParseMessage;
            webSocketConnection.onClose = () => { Console.WriteLine("[Server]: Connection closed"); };
            webSocketConnection.onError = () => { Console.WriteLine("[Server]: Connection error encountered"); };

            Console.WriteLine("[Server]: New connection established successfully");
        }

        private static async void ParseMessage(string message)
        {
            Console.WriteLine($"[Client]: {message}");

            //if (message.Contains("UpdateDataRequest"))
            //{
            //    await SendDevices();
            //}
        }

    }
}