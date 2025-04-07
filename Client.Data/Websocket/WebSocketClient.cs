using ClientServer.Shared.WebSocket;
using System.Net.WebSockets;
using System.Text;

namespace Client.Data.Websocket
{
    public static class WebSocketClient
    {
        public static event Action<string>? OnMessage;
        public static event Action? OnDataArrived;

        public static WebSocketConnection CurrentConnection { get; private set; } = default!;

        public static async Task<WebSocketConnection> Connect(Uri peer, Action<string> log)
        {
            ClientWebSocket m_ClientWebSocket = new ClientWebSocket();

            await m_ClientWebSocket.ConnectAsync(peer, CancellationToken.None);

            switch (m_ClientWebSocket.State)
            {
                case WebSocketState.Open:
                    log($"Opening WebSocket connection to remote server {peer}");
                    WebSocketConnection _socket = new ClintWebSocketConnection(m_ClientWebSocket, peer, log);
                    CurrentConnection = _socket;
                    CurrentConnection.onMessage = (message) => OnMessage?.Invoke(message);
                    return _socket;

                default:
                    log?.Invoke($"Cannot connect to remote node status {m_ClientWebSocket.State}");
                    throw new WebSocketException($"Cannot connect to remote node status {m_ClientWebSocket.State}");
            }
        }

        public static async Task Disconnect()
        {
            await CurrentConnection.DisconnectAsync();

            CurrentConnection = null!;
        }

        private class ClintWebSocketConnection : WebSocketConnection
        {
            private readonly ClientWebSocket m_ClientWebSocket;
            private readonly Uri m_Peer;
            private readonly Action<string> m_Log;

            public ClintWebSocketConnection(ClientWebSocket clientWebSocket, Uri m_Peer, Action<string> log)
            {
                m_ClientWebSocket = clientWebSocket ?? throw new ArgumentNullException(nameof(clientWebSocket));
                this.m_Peer = m_Peer ?? throw new ArgumentNullException(nameof(m_Peer));
                m_Log = log ?? throw new ArgumentNullException(nameof(log));
                Task.Factory.StartNew(ClientMessageLoop, TaskCreationOptions.LongRunning);
            }

            protected override Task SendTask(string message)
            {
                return m_ClientWebSocket.SendAsync(message.GetArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None);
            }

            private volatile bool isRunning = true;

            public override Task DisconnectAsync()
            {
                isRunning = false;

                if (!isRunning)
                    return m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown procedure started", CancellationToken.None);

                return Task.CompletedTask;
            }

            public override string ToString()
            {
                return m_Peer.ToString();
            }

            private async Task ClientMessageLoop()
            {
                try
                {
                    byte[] buffer = new byte[1024 * 24]; // 24KB buffer

                    while (isRunning)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
                        WebSocketReceiveResult result = await m_ClientWebSocket.ReceiveAsync(segment, CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            m_Log($"Received close message from server: {result.CloseStatus} - {result.CloseStatusDescription}");
                            isRunning = false;
                            onClose?.Invoke();
                            return;
                        }

                        int count = result.Count;
                        while (!result.EndOfMessage)
                        {
                            if (count >= buffer.Length)
                            {
                                m_Log("Message too long, closing connection.");
                                await m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Message too long", CancellationToken.None);
                                onClose?.Invoke();
                                return;
                            }
                            segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                            result = await m_ClientWebSocket.ReceiveAsync(segment, CancellationToken.None);
                            count += result.Count;
                        }

                        string message = Encoding.UTF8.GetString(buffer, 0, count);
                        onMessage?.Invoke(message);
                    }
                }
                catch (WebSocketException e)
                {
                    Console.WriteLine($"Caught web socket exception {e.Message}");
                }
                finally
                {
                    if (m_ClientWebSocket.State != WebSocketState.Closed)
                    {
                        m_Log("Ensuring WebSocket is closed.");
                        await m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Final cleanup", CancellationToken.None);
                    }
                    onClose?.Invoke();
                }
            }
        }

        private static ArraySegment<byte> GetArraySegment(this string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            return new ArraySegment<byte>(buffer);
        }
    }
}
