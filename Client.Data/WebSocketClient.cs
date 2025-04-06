using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Client.Data.Generated; // klasy XSD
using Shared.Communication;

namespace Client.Data
{
    public class WebSocketClient : IDisposable
    {
        private ClientWebSocket _clientWebSocket;
        private readonly Uri _serverUri;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<Generated.WebSocketMessage?>> _pendingRequests; // Do dopasowywania odpowiedzi do żądań

        // Zdarzenie do powiadamiania o otrzymaniu wiadomości "push" (np. aktualizacji) z serwera
        public event Func<Generated.WebSocketMessage, Task>? NotificationReceived;
        // Zdarzenie informujące o stanie połączenia
        public event Action<WebSocketState>? ConnectionStateChanged;

        public WebSocketClient(Uri serverUri)
        {
            _serverUri = serverUri;
            _clientWebSocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
            _pendingRequests = new ConcurrentDictionary<Guid, TaskCompletionSource<Generated.WebSocketMessage?>>();
        }

        public WebSocketState State => _clientWebSocket?.State ?? WebSocketState.None;

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (State == WebSocketState.Open)
            {
                Console.WriteLine("Already connected.");
                return;
            }

            // Jeśli poprzednie połączenie nie zostało poprawnie zamknięte/zresetowane
            if (_clientWebSocket.State != WebSocketState.None && _clientWebSocket.State != WebSocketState.Closed)
            {
                Console.WriteLine($"WebSocket is in state {_clientWebSocket.State}. Attempting to abort and recreate.");
                try { _clientWebSocket.Abort(); } catch { }
                _clientWebSocket.Dispose();
                _clientWebSocket = new ClientWebSocket();
                _cancellationTokenSource = new CancellationTokenSource(); // Resetuj token dla nowego połączenia
            }
            else if (_clientWebSocket.State == WebSocketState.Closed)
            {
                _clientWebSocket.Dispose();
                _clientWebSocket = new ClientWebSocket();
                _cancellationTokenSource = new CancellationTokenSource(); // Resetuj token
            }


            Console.WriteLine($"Connecting to {_serverUri}...");
            ConnectionStateChanged?.Invoke(WebSocketState.Connecting);
            try
            {
                // Połącz główny token z tokenem z metody
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
                await _clientWebSocket.ConnectAsync(_serverUri, linkedCts.Token);
                Console.WriteLine($"Connected to {_serverUri}. State: {_clientWebSocket.State}");
                ConnectionStateChanged?.Invoke(_clientWebSocket.State);

                // Uruchom pętlę nasłuchującą w tle
                _ = Task.Run(ReceiveLoopAsync, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Connection attempt cancelled.");
                ConnectionStateChanged?.Invoke(_clientWebSocket.State);
                throw; // Rzuć dalej, aby kod wywołujący wiedział o anulowaniu
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket connection error: {ex.Message} (Status: {ex.WebSocketErrorCode})");
                ConnectionStateChanged?.Invoke(_clientWebSocket.State);
                throw; // Rzuć dalej, aby UI mogło zareagować
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
                ConnectionStateChanged?.Invoke(_clientWebSocket.State);
                throw;
            }
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[1024 * 8]; // Większy bufor może być potrzebny dla XML
            var messageBuilder = new StringBuilder();

            try
            {
                while (State == WebSocketState.Open && !_cancellationTokenSource.IsCancellationRequested)
                {
                    messageBuilder.Clear();
                    WebSocketReceiveResult receiveResult;
                    do
                    {
                        // Użyj tokena z _cancellationTokenSource do anulowania pętli
                        receiveResult = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

                        if (receiveResult.MessageType == WebSocketMessageType.Close)
                        {
                            Console.WriteLine($"Server initiated close. Status: {receiveResult.CloseStatus}, Description: {receiveResult.CloseStatusDescription}");
                            // Odpowiedz zamknięciem i zakończ pętlę
                            if (State == WebSocketState.CloseReceived)
                            {
                                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client acknowledging close", CancellationToken.None);
                            }
                            ConnectionStateChanged?.Invoke(WebSocketState.Closed);
                            return; // Zakończ pętlę
                        }

                        if (receiveResult.MessageType == WebSocketMessageType.Text)
                        {
                            messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, receiveResult.Count));
                        }
                        else
                        {
                            Console.WriteLine($"Received unexpected message type: {receiveResult.MessageType}");
                            // Ignoruj lub obsłuż inaczej
                        }

                    } while (!receiveResult.EndOfMessage); // Kontynuuj odbieranie, jeśli wiadomość jest podzielona

                    string receivedXml = messageBuilder.ToString();
                    if (!string.IsNullOrEmpty(receivedXml))
                    {
                        Console.WriteLine($"Received XML ({receivedXml.Length} bytes): {receivedXml.Substring(0, Math.Min(receivedXml.Length, 100))}...");
                        Generated.WebSocketMessage? message = XmlSerializationHelper.DeserializeMessage(receivedXml);

                        if (message != null)
                        {
                            await HandleReceivedMessageAsync(message);
                        }
                        else
                        {
                            Console.WriteLine("Failed to deserialize received message.");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Receive loop cancelled.");
            }
            catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely || _clientWebSocket.State != WebSocketState.Open)
            {
                Console.WriteLine($"WebSocket connection lost: {ex.Message} (State: {State})");
                ConnectionStateChanged?.Invoke(State); // Zaktualizuj stan
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in receive loop: {ex.Message}\n{ex.StackTrace}");
                ConnectionStateChanged?.Invoke(State); // Zaktualizuj stan (prawdopodobnie Aborted lub Closed)
            }
            finally
            {
                Console.WriteLine("Receive loop ended.");
                // Upewnij się, że stan jest aktualizowany, jeśli pętla się zakończyła
                if (State != WebSocketState.Closed)
                {
                    ConnectionStateChanged?.Invoke(State);
                }
                // Rozważ próbę ponownego połączenia tutaj lub w logice wyższego poziomu
            }
        }

        private async Task HandleReceivedMessageAsync(Generated.WebSocketMessage message)
        {
            // Sprawdź, czy to odpowiedź na jedno z oczekujących żądań
            // Możemy dodać CorrelationId do WebSocketMessage, aby to ułatwić.
            // Bez tego, musimy polegać na typie odpowiedzi pasującym do typu żądania
            // Przykład: Prosty mechanizm dopasowania (wymaga rozbudowy dla wielu żądań tego samego typu!)

            bool handled = false;
            // Tutaj potrzebujemy bardziej zaawansowanego mechanizmu korelacji
            // np. wysyłając Guid z żądaniem i oczekując go w odpowiedzi.
            // Na razie uproszczenie: załóżmy, że typ odpowiedzi wystarczy do znalezienia oczekującego TCS
            Guid? requestKey = FindPendingRequestKey(message.MessageType);

            if (requestKey.HasValue && _pendingRequests.TryRemove(requestKey.Value, out var tcs))
            {
                Console.WriteLine($"Received response for request {requestKey.Value}, Type: {message.MessageType}");
                tcs.TrySetResult(message);
                handled = true;
            }

            // Jeśli nie jest to odpowiedź na żądanie, traktuj jako powiadomienie push
            if (!handled)
            {
                // Czy to typ powiadomienia?
                if (IsNotificationType(message.MessageType))
                {
                    Console.WriteLine($"Received notification: {message.MessageType}");
                    if (NotificationReceived != null)
                    {
                        try
                        {
                            // Wywołaj delegata asynchronicznie
                            await NotificationReceived.Invoke(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error handling notification: {ex.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Received unexpected message (not a response or known notification): {message.MessageType}");
                }
            }
        }

        // Uproszczony przykład, jak można by znaleźć klucz (wymaga poprawy!)
        private Guid? FindPendingRequestKey(MessageType responseType)
        {
            // Tu powinna być logika mapowania typu odpowiedzi na typ żądania i znajdowania pasującego TCS
            // Np. GetAllHeroesResponse pasuje do GetAllHeroesRequest
            MessageType requestType = MapResponseToRequestType(responseType);
            foreach (var pair in _pendingRequests)
            {
                // Tu potrzebujemy informacji o tym, jakiego typu było żądanie powiązane z kluczem 'pair.Key'
                // Bez tego, ten mechanizm jest zawodny.
                // Lepsze rozwiązanie: wysyłaj Guid jako CorrelationId w WebSocketMessage.
            }
            // Na razie zwracamy null, co spowoduje traktowanie jako powiadomienie.
            // Trzeba to zaimplementować poprawnie z CorrelationId.
            return null;
        }

        private MessageType MapResponseToRequestType(MessageType responseType)
        {
            // Odwrotne mapowanie do MapRequestToResponseType w handlerze serwera
            // ... implementacja ...
            return MessageType.ErrorResponse; // Placeholder
        }


        // Sprawdza, czy typ wiadomości to powiadomienie push
        private bool IsNotificationType(MessageType messageType)
        {
            return messageType.ToString().EndsWith("Notification");
            // return messageType switch {
            //    MessageType.HeroUpdateNotification => true,
            //    MessageType.ItemUpdateNotification => true,
            //    MessageType.OrderUpdateNotification => true,
            //    // ... inne powiadomienia
            //    _ => false
            //};
        }

        // Metoda do wysyłania żądania i oczekiwania na odpowiedź z CorrelationId
        public async Task<Generated.WebSocketMessage?> SendRequestAsync(Generated.WebSocketMessage requestMessage, CancellationToken cancellationToken = default)
        {
            if (State != WebSocketState.Open)
            {
                throw new InvalidOperationException("WebSocket is not connected.");
            }

            // TODO: Dodaj CorrelationId do WebSocketMessage, jeśli jeszcze go nie ma
            // Guid correlationId = Guid.NewGuid();
            // requestMessage.CorrelationId = correlationId; // Załóżmy, że dodaliśmy pole CorrelationId

            var tcs = new TaskCompletionSource<Generated.WebSocketMessage?>();
            Guid correlationId = Guid.NewGuid(); // Klucz do śledzenia żądania

            if (!_pendingRequests.TryAdd(correlationId, tcs))
            {
                // Mało prawdopodobne z Guid, ale możliwe
                throw new InvalidOperationException("Failed to register pending request.");
            }

            string xmlMessage = XmlSerializationHelper.SerializeMessage(requestMessage);
            if (string.IsNullOrEmpty(xmlMessage))
            {
                _pendingRequests.TryRemove(correlationId, out _); // Usuń z oczekujących
                throw new InvalidOperationException("Failed to serialize request message.");
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(xmlMessage);
            Console.WriteLine($"Sending request ({correlationId}): {requestMessage.MessageType} ({messageBytes.Length} bytes)");

            try
            {
                // Połącz tokeny: globalny, metody, timeout
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // Timeout na odpowiedź
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    _cancellationTokenSource.Token, // Globalny token klienta
                    cancellationToken,            // Token przekazany do metody
                    timeoutCts.Token             // Token timeoutu
                );

                await _clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, linkedCts.Token);

                // Czekaj na odpowiedź lub timeout/anulowanie
                using (linkedCts.Token.Register(() => tcs.TrySetCanceled(linkedCts.Token)))
                {
                    // Czekaj na wynik ustawiony przez ReceiveLoopAsync lub anulowanie
                    var result = await tcs.Task;
                    Console.WriteLine($"Received response for request {correlationId}.");
                    return result; // Zwróć otrzymaną wiadomość
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Request {correlationId} ({requestMessage.MessageType}) cancelled or timed out.");
                _pendingRequests.TryRemove(correlationId, out _); // Usuń z oczekujących
                throw; // Rzuć dalej, aby kod wywołujący wiedział
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending request {correlationId} ({requestMessage.MessageType}): {ex.Message}");
                _pendingRequests.TryRemove(correlationId, out _); // Usuń z oczekujących
                throw; // Rzuć dalej
            }
        }


        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (State == WebSocketState.Open || State == WebSocketState.CloseReceived)
            {
                Console.WriteLine("Disconnecting...");
                ConnectionStateChanged?.Invoke(WebSocketState.CloseSent);
                _cancellationTokenSource.Cancel(); // Sygnalizuj zakończenie pętli

                try
                {
                    // Użyj tokena przekazanego + timeout
                    using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                    await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client initiated disconnect", linkedCts.Token);
                    Console.WriteLine("WebSocket closed gracefully.");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Disconnect operation cancelled or timed out. Aborting.");
                    _clientWebSocket.Abort(); // Wymuś zamknięcie
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during disconnect: {ex.Message}. Aborting.");
                    _clientWebSocket.Abort(); // Wymuś zamknięcie
                }
                ConnectionStateChanged?.Invoke(State); // Zaktualizuj stan
            }
            else
            {
                Console.WriteLine($"Cannot disconnect, state is: {State}");
            }
            // Wyczyść oczekujące żądania
            foreach (var pair in _pendingRequests)
            {
                pair.Value.TrySetCanceled(); // Anuluj wszystkie oczekujące żądania
            }
            _pendingRequests.Clear();
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing WebSocketClient...");
            // Upewnij się, że rozłączono przed Dispose
            if (State == WebSocketState.Open)
            {
                // Użyj synchronicznego oczekiwania lub metody Fire-and-forget
                // Task.Run(() => DisconnectAsync()).Wait(); // Może zakleszczyć!
                // Lepsze jest zapewnienie wywołania DisconnectAsync przed Dispose
                Console.WriteLine("Warning: Dispose called while WebSocket was still open. Call DisconnectAsync first.");
                _clientWebSocket.Abort(); // W ostateczności
            }
            _cancellationTokenSource.Cancel(); // Anuluj wszelkie operacje
            _clientWebSocket?.Dispose();
            _cancellationTokenSource.Dispose();
            Console.WriteLine("WebSocketClient disposed.");
        }
    }
}