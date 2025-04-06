using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Server.Data.Generated; // Dla WebSocketMessage (jeśli używane do powiadomień)
using Shared.Communication; // Dla WebSocketMessage

namespace Server.Data
{
    public class WebSocketServer : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly IServerLogicApi _logicApi;
        private readonly ConcurrentDictionary<Guid, ClientConnectionHandler> _clients = new ConcurrentDictionary<Guid, ClientConnectionHandler>();
        private CancellationTokenSource _serverCancellationTokenSource = new CancellationTokenSource();

        public WebSocketServer(string uriPrefix, IServerLogicApi logicApi)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("HttpListener is not supported on this platform.");
            }
            _listener = new HttpListener();
            _listener.Prefixes.Add(uriPrefix); // Np. "http://localhost:8080/ws/"
            _logicApi = logicApi ?? throw new ArgumentNullException(nameof(logicApi));
        }

        public async Task StartAsync()
        {
            if (_listener.IsListening) return;

            _listener.Start();
            Console.WriteLine($"Server listening on {_listener.Prefixes.First()}...");

            try
            {
                while (!_serverCancellationTokenSource.IsCancellationRequested)
                {
                    HttpListenerContext context = await _listener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        // Akceptuj połączenie WebSocket asynchronicznie
                        _ = ProcessWebSocketRequestAsync(context); // Uruchom i zapomnij (lub użyj Task.Run)
                    }
                    else
                    {
                        Console.WriteLine("Received non-WebSocket request.");
                        context.Response.StatusCode = 400; // Bad Request
                        context.Response.Close();
                    }
                }
            }
            catch (HttpListenerException ex) when (ex.ErrorCode == 995) // ERROR_OPERATION_ABORTED
            {
                Console.WriteLine("HttpListener stopped listening (operation aborted).");
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("HttpListener has been disposed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error in listen loop: {ex.Message}");
                // TODO: Logowanie błędu
            }
            finally
            {
                StopInternal();
            }
        }

        private async Task ProcessWebSocketRequestAsync(HttpListenerContext context)
        {
            WebSocketContext? wsContext = null;
            try
            {
                wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                Guid clientId = Guid.NewGuid();
                var webSocket = wsContext.WebSocket;

                var handler = new ClientConnectionHandler(webSocket, clientId, _logicApi, RemoveClient);
                if (_clients.TryAdd(clientId, handler))
                {
                    Console.WriteLine($"WebSocket connection established for client: {clientId}");
                    // Rozpocznij obsługę klienta w tle
                    await handler.StartHandlingAsync(); // Czekaj na zakończenie obsługi tego klienta
                }
                else
                {
                    // Nie powinno się zdarzyć z nowym Guid, ale na wszelki wypadek
                    Console.WriteLine($"Failed to add client handler for {clientId}. Closing connection.");
                    await webSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Failed to register client", CancellationToken.None);
                    webSocket.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting WebSocket connection: {ex.Message}");
                context.Response.StatusCode = 500;
                context.Response.Close(); // Zamknij odpowiedź HTTP, jeśli WebSocket nie został zaakceptowany
                wsContext?.WebSocket?.Dispose(); // Upewnij się, że zasoby WebSocket są zwolnione
            }
        }

        private void RemoveClient(Guid clientId)
        {
            if (_clients.TryRemove(clientId, out var handler))
            {
                Console.WriteLine($"Client {clientId} removed from active connections.");
                // Handler powinien sam posprzątać swoje zasoby (WebSocket.Dispose)
            }
        }

        public void Stop()
        {
            Console.WriteLine("Stopping server...");
            _serverCancellationTokenSource.Cancel(); // Sygnalizuj zakończenie pętli nasłuchującej i handlerom
            StopInternal();
            Console.WriteLine("Server stopped.");
        }

        private void StopInternal()
        {
            if (_listener.IsListening)
            {
                _listener.Stop(); // Przestań akceptować nowe połączenia
                _listener.Close(); // Zwolnij zasoby HttpListener
            }

            // Zamknij wszystkie istniejące połączenia klientów
            // Iteracja po kopi wartości, aby uniknąć problemów ze współbieżnością podczas usuwania
            foreach (var handler in _clients.Values.ToList())
            {
                // Handler powinien sam zamknąć połączenie po otrzymaniu sygnału Cancel
                // Możemy tu dodać wymuszenie, ale CancellationTokenSource w handlerze powinien wystarczyć.
            }
            _clients.Clear();
        }

        // Metoda do rozsyłania powiadomień do wszystkich (lub wybranych) klientów
        public async Task BroadcastNotificationAsync(Generated.WebSocketMessage notification, Guid? excludeClientId = null)
        {
            Console.WriteLine($"Broadcasting notification: {notification.MessageType}");
            // Serializuj raz, wyślij do wielu
            string xmlMessage = XmlSerializationHelper.Serialize(notification);
            if (string.IsNullOrEmpty(xmlMessage))
            {
                Console.WriteLine("Failed to serialize broadcast message.");
                return;
            }
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(xmlMessage);
            var segment = new ArraySegment<byte>(messageBytes);

            List<Task> sendTasks = new List<Task>();

            foreach (var clientEntry in _clients)
            {
                if (excludeClientId.HasValue && clientEntry.Key == excludeClientId.Value)
                {
                    continue; // Pomiń klienta, który wywołał akcję
                }

                // Używamy bezpośredniego dostępu do WebSocket przez handlera (jeśli jest publiczny)
                // lub dodajemy metodę SendAsync do samego handlera
                // Załóżmy, że handler ma publiczną metodę SendRawAsync lub używa SendMessageAsync
                // sendTasks.Add(clientEntry.Value.SendMessageAsync(notification)); // Bezpieczniejsze, ale wymaga logiki w handlerze

                // LUB: Jeśli WebSocket jest dostępny (mniej bezpieczne enkapsulacyjnie)
                try
                {
                    // To jest tylko przykład, lepiej zrobić to przez dedykowaną metodę w Handlerze
                    // if (clientEntry.Value._webSocket.State == WebSocketState.Open) {
                    //     sendTasks.Add(clientEntry.Value._webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None));
                    // }

                    // Preferowane podejście: użyj metody SendNotificationAsync w handlerze
                    sendTasks.Add(clientEntry.Value.SendNotificationAsync(notification));

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error preparing broadcast send for client {clientEntry.Key}: {ex.Message}");
                }
            }

            await Task.WhenAll(sendTasks);
            Console.WriteLine($"Broadcast finished for {notification.MessageType}. Sent to {sendTasks.Count} clients.");
        }


        public void Dispose()
        {
            Stop();
            _serverCancellationTokenSource.Dispose();
            // Listener jest zamykany w StopInternal()
        }
    }
}