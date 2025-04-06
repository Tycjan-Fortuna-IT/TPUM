using System;
using System.Collections.Concurrent; // Dla wątkowo-bezpiecznej kolekcji klientów
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.Communication; // Dla XmlSerializationHelper
using Shared.DataModels; // Używamy bezpośrednio klas z Shared

namespace Server.Data;

public class WebSocketServer : IDisposable
{
    private HttpListener _listener;
    private readonly string _url;
    private readonly IServerLogicRouter _logicRouter; // Interfejs do warstwy logiki
    private readonly ConcurrentDictionary<Guid, WebSocket> _clients = new ConcurrentDictionary<Guid, WebSocket>();
    private CancellationTokenSource _cts = new CancellationTokenSource(); // Główny token anulowania dla serwera

    public WebSocketServer(string hostname, int port, IServerLogicRouter logicRouter)
    {
        // Url musi zawierać ukośnik na końcu, np. "http://localhost:8080/"
        // Użyj + lub * zamiast localhost, aby nasłuchiwać na wszystkich interfejsach (może wymagać uprawnień admina lub konfiguracji urlacl)
        // Użycie '*' jest zalecane dla .NET Core/5+
        _url = $"http://{hostname}:{port}/";
        _listener = new HttpListener();
        _listener.Prefixes.Add(_url);
        _logicRouter = logicRouter;
    }

    public async Task StartAsync()
    {
        if (_cts.IsCancellationRequested) // Jeśli serwer był już zatrzymany i ponownie uruchamiany
        {
            _cts = new CancellationTokenSource();
        }

        _listener.Start();
        Console.WriteLine($"WebSocket Server listening on {_url}");

        try
        {
            // Pętla akceptująca połączenia
            while (!_cts.Token.IsCancellationRequested)
            {
                // Asynchroniczne oczekiwanie na przychodzące żądanie HTTP
                HttpListenerContext listenerContext = await _listener.GetContextAsync();

                // Sprawdzenie, czy żądanie jest próbą nawiązania połączenia WebSocket
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    // Rozpocznij obsługę połączenia WebSocket w tle, aby nie blokować pętli akceptującej
                    _ = ProcessWebSocketRequestAsync(listenerContext, _cts.Token);
                }
                else
                {
                    // Odpowiedz na zwykłe żądania HTTP (np. informacją, że to endpoint WebSocket)
                    listenerContext.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                    listenerContext.Response.StatusDescription = "Not a WebSocket request";
                    listenerContext.Response.Close();
                }
            }
        }
        catch (HttpListenerException ex) when (ex.ErrorCode == 995) // Operacja przerwana (Listener stop)
        {
            Console.WriteLine("HttpListener stopped gracefully.");
        }
        catch (ObjectDisposedException) // Listener został zdisposowany
        {
            Console.WriteLine("HttpListener has been disposed.");
        }
        catch (OperationCanceledException) // GetContextAsync anulowane przez _cts.Cancel()
        {
            Console.WriteLine("Server listening loop cancelled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Server error in listening loop: {ex.GetType().Name} - {ex.Message}");
            // W zależności od błędu, można rozważyć zatrzymanie serwera
        }
        finally
        {
            if (_listener.IsListening)
            {
                _listener.Stop(); // Zatrzymaj nasłuchiwanie
            }
            Console.WriteLine("Server has stopped listening.");
        }
    }

    // Metoda obsługująca pojedyncze połączenie WebSocket
    private async Task ProcessWebSocketRequestAsync(HttpListenerContext listenerContext, CancellationToken serverStoppingToken)
    {
        WebSocketContext? webSocketContext = null;
        WebSocket? webSocket = null;
        Guid clientId = Guid.NewGuid(); // Unikalny identyfikator dla tego połączenia
        // Token specyficzny dla tego połączenia, powiązany z głównym tokenem serwera
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(serverStoppingToken);
        CancellationToken connectionToken = linkedCts.Token;


        try
        {
            // Akceptacja połączenia WebSocket
            webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
            webSocket = webSocketContext.WebSocket;

            // Dodanie klienta do słownika
            if (_clients.TryAdd(clientId, webSocket))
            {
                Console.WriteLine($"Client Connected: {clientId} from {listenerContext.Request.RemoteEndPoint}");

                // Rozpocznij pętlę odbioru dla tego klienta, używając tokenu połączenia
                await ReceiveLoopAsync(clientId, webSocket, connectionToken);
            }
            else
            {
                // Powinno być bardzo mało prawdopodobne z Guid.NewGuid()
                Console.WriteLine($"CRITICAL: Failed to add client {clientId} due to Guid collision. Closing connection.");
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.InternalServerError, "Failed to register client", CancellationToken.None);
                webSocket.Dispose();
            }
        }
        catch (OperationCanceledException) when (serverStoppingToken.IsCancellationRequested)
        {
            // Serwer jest zatrzymywany przed pełnym nawiązaniem połączenia
            Console.WriteLine($"Connection attempt from {listenerContext.Request.RemoteEndPoint} cancelled due to server shutdown.");
            listenerContext.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable; // 503
            listenerContext.Response.Close();
            webSocket?.Dispose(); // Upewnij się, że zasób jest zwolniony
        }
        catch (Exception ex)
        {
            // Błąd podczas akceptacji połączenia lub wczesnej fazy
            Console.WriteLine($"Error accepting WebSocket connection for {clientId}: {ex.GetType().Name} - {ex.Message}");
            // Sprawdź, czy odpowiedź HTTP nie została już wysłana
            listenerContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
            listenerContext.Response.Close();
            webSocket?.Dispose(); // Upewnij się, że zasób jest zwolniony
        }
        finally
        {
            // Kod wykonywany ZAWSZE po zakończeniu obsługi klienta (normalnym lub przez błąd)
            // Usuń klienta ze słownika, jeśli nadal tam jest
            if (_clients.TryRemove(clientId, out var removedSocket))
            {
                Console.WriteLine($"Client Disconnected: {clientId}");
                // Zamknij i zwolnij zasoby socketu, jeśli jeszcze nie zostały
                await CloseAndDisposeWebSocketAsync(removedSocket, WebSocketCloseStatus.NormalClosure, "Client session ended");
            }
            // Anuluj token specyficzny dla tego połączenia, jeśli jeszcze nie został
            // (choć zazwyczaj już będzie anulowany przez błąd lub normalne zakończenie)
            if (!linkedCts.IsCancellationRequested)
            {
                linkedCts.Cancel();
            }
            Console.WriteLine($"Finished processing for client {clientId}.");
            // TODO: Wywołaj logikę czyszczenia sesji związanej z clientId, jeśli jest potrzebna
        }
    }

    // Pętla odbierająca wiadomości od pojedynczego klienta
    private async Task ReceiveLoopAsync(Guid clientId, WebSocket webSocket, CancellationToken connectionToken)
    {
        // Bufor o rozmiarze zalecanym przez Microsoft
        var buffer = WebSocket.CreateServerBuffer(4 * 1024);

        try
        {
            // Dopóki socket jest otwarty i nie zażądano anulowania połączenia
            while (webSocket.State == WebSocketState.Open && !connectionToken.IsCancellationRequested)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        // Odbierz fragment wiadomości
                        // ReceiveAsync rzuci OperationCanceledException, jeśli connectionToken zostanie anulowany
                        result = await webSocket.ReceiveAsync(buffer, connectionToken);

                        // Sprawdź, czy klient zainicjował zamknięcie połączenia
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            Console.WriteLine($"Client {clientId} initiated close. Status: {result.CloseStatus}, Description: {result.CloseStatusDescription}");
                            // Potwierdź zamknięcie, jeśli stan na to pozwala
                            if (webSocket.State == WebSocketState.CloseReceived || webSocket.State == WebSocketState.Open)
                            {
                                await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Acknowledging client close", CancellationToken.None); // Użyj CancellationToken.None, bo klient już zamknął swoje wyjście
                            }
                            return; // Zakończ pętlę odbioru dla tego klienta
                        }

                        // Obsłuż tylko wiadomości tekstowe
                        if (result.MessageType != WebSocketMessageType.Text)
                        {
                            Console.WriteLine($"Client {clientId} sent non-text message type: {result.MessageType}. Closing connection.");
                            await CloseAndDisposeWebSocketAsync(webSocket, WebSocketCloseStatus.InvalidMessageType, "Only text messages are supported");
                            return; // Zakończ pętlę
                        }

                        // Dopisz odebrany fragment do strumienia pamięci
                        ms.Write(buffer.Array!, buffer.Offset, result.Count);

                    } while (!result.EndOfMessage); // Kontynuuj, jeśli wiadomość nie została jeszcze w pełni odebrana

                    ms.Seek(0, System.IO.SeekOrigin.Begin);

                    if (ms.Length == 0)
                    {
                        Console.WriteLine($"Client {clientId} sent an empty message.");
                        continue; // Ignoruj puste wiadomości
                    }

                    // Odczytaj pełną wiadomość jako string (XML)
                    string xmlMessage = Encoding.UTF8.GetString(ms.ToArray());
                    Console.WriteLine($"Received XML from {clientId} ({ms.Length} bytes): {xmlMessage.Substring(0, Math.Min(xmlMessage.Length, 200))}..."); // Loguj tylko początek dla zwięzłości

                    // Uruchom obsługę wiadomości asynchronicznie w tle, aby nie blokować pętli odbioru
                    // Przekaż clientId, webSocket i wiadomość
                    // UWAGA: Jeśli HandleMessageAsync ma operacje długotrwałe, rozważ użycie Task.Run lub kolejki przetwarzania
                    _ = HandleMessageAsync(clientId, webSocket, xmlMessage, connectionToken);

                } // using MemoryStream - zwolnienie pamięci strumienia
            } // while open and not cancelled
        }
        catch (OperationCanceledException) when (connectionToken.IsCancellationRequested)
        {
            // Oczekiwane anulowanie (np. zatrzymanie serwera lub błąd w innym miejscu)
            Console.WriteLine($"Receive loop for client {clientId} cancelled.");
        }
        catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely ||
                                           webSocket.State == WebSocketState.Aborted ||
                                           webSocket.State == WebSocketState.Closed)
        {
            // Połączenie zostało nagle przerwane przez klienta lub sieć
            Console.WriteLine($"WebSocket connection for client {clientId} closed unexpectedly: {ex.Message} (State: {webSocket.State})");
            // Pętla zakończy się naturalnie, a blok finally w ProcessWebSocketRequestAsync posprząta
        }
        catch (Exception ex)
        {
            // Inny, nieoczekiwany błąd w pętli odbioru
            Console.WriteLine($"CRITICAL Error in receive loop for client {clientId}: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine(ex.StackTrace); // Logowanie śladu stosu
            // Spróbuj zamknąć połączenie z powodu błędu
            await CloseAndDisposeWebSocketAsync(webSocket, WebSocketCloseStatus.InternalServerError, "Internal server error during receive");
            // Pętla zostanie przerwana, finally w ProcessWebSocketRequestAsync posprząta
        }
        finally
        {
            Console.WriteLine($"Receive loop for client {clientId} finished. Final State: {webSocket.State}");
            // Sprzątanie (usuwanie z _clients, zamykanie i dispose) odbywa się w bloku finally metody ProcessWebSocketRequestAsync
        }
    }

    // Metoda przetwarzająca pojedynczą wiadomość i wysyłająca odpowiedź
    private async Task HandleMessageAsync(Guid clientId, WebSocket webSocket, string xmlMessage, CancellationToken connectionToken)
    {
        // Sprawdź, czy połączenie jest nadal aktywne przed przetwarzaniem
        if (webSocket.State != WebSocketState.Open || connectionToken.IsCancellationRequested)
        {
            Console.WriteLine($"Skipping message handling for {clientId}; connection is not open or cancelled.");
            return;
        }

        Request? request = null;
        Response response;

        try
        {
            // 1. Deserializacja XML do obiektu Request (z Shared.DataModels)
            request = XmlSerializationHelper.Deserialize<Request>(xmlMessage);

            if (request == null)
            {
                Console.WriteLine($"Failed to deserialize request from {clientId}: {xmlMessage.Substring(0, Math.Min(xmlMessage.Length, 100))}...");
                await SendErrorResponseAsync(webSocket, Guid.Empty, "Invalid request format.", connectionToken);
                return;
            }

            // Przygotuj domyślną odpowiedź z poprawnym CorrelationId
            response = new Response { CorrelationId = request.RequestId, Success = false };

            // 2. Przekazanie obiektu (z Shared.DataModels) do Server.Logic przez router
            //    LogicRouter oczekuje teraz klas z Shared.DataModels.
            response = await _logicRouter.RouteRequestAsync(request); // Logika biznesowa i mapowania wewnątrz routera/logiki
            response.CorrelationId = request.RequestId; // Upewnij się, że ID jest zachowane
        }
        catch (Exception ex)
        {
            // Błąd podczas deserializacji, przetwarzania w logice lub tworzenia odpowiedzi
            Guid correlationId = request?.RequestId ?? Guid.Empty;
            Console.WriteLine($"Error processing request {correlationId} ({request?.Operation}) from {clientId}: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine(ex.StackTrace); // Loguj ślad stosu dla błędów wewnętrznych
            response = new Response // Stwórz obiekt odpowiedzi o błędzie
            {
                CorrelationId = correlationId,
                Success = false,
                ErrorMessage = $"Internal server error occurred processing your request. Details: {ex.Message}" // Można uprościć dla klienta
            };
        }

        try
        {
            // 3. Serializacja obiektu Response (z Shared.DataModels) do XML
            string xmlResponse = XmlSerializationHelper.Serialize(response);

            // 4. Wysłanie odpowiedzi XML do klienta
            await SendMessageAsync(webSocket, xmlResponse, connectionToken);
        }
        catch (Exception ex)
        {
            // Błąd podczas serializacji lub wysyłania odpowiedzi
            Console.WriteLine($"Failed to send response for CorrelationId {response.CorrelationId} to {clientId}: {ex.GetType().Name} - {ex.Message}");
            // Trudno tutaj wiele zrobić, poza zalogowaniem błędu. Połączenie mogło zostać zamknięte.
        }
    }

    // Metoda do wysyłania wiadomości tekstowej do klienta
    private async Task SendMessageAsync(WebSocket webSocket, string message, CancellationToken cancellationToken)
    {
        if (webSocket.State != WebSocketState.Open)
        {
            Console.WriteLine($"Cannot send message, WebSocket is not in Open state (State: {webSocket.State}).");
            return; // Nie próbuj wysyłać, jeśli socket nie jest otwarty
        }

        if (string.IsNullOrEmpty(message))
        {
            Console.WriteLine("Attempted to send an empty message.");
            return;
        }

        Console.WriteLine($"Sending XML ({message.Length} bytes): {message.Substring(0, Math.Min(message.Length, 200))}..."); // Loguj początek

        byte[] buffer = Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(buffer);

        try
        {
            // Wyślij wiadomość, używając tokenu anulowania specyficznego dla połączenia
            await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine($"Send operation cancelled for message: {message.Substring(0, Math.Min(message.Length, 50))}...");
        }
        catch (WebSocketException ex)
        {
            // Błąd podczas wysyłania, prawdopodobnie związany ze stanem połączenia
            Console.WriteLine($"WebSocketException sending message: {ex.WebSocketErrorCode} - {ex.Message} (State: {webSocket.State})");
            // Jeśli błąd wynika z zamkniętego połączenia, to jest to mniej krytyczne
            if (webSocket.State != WebSocketState.Open && webSocket.State != WebSocketState.CloseSent)
            {
                // Połączenie zostało już zamknięte lub jest w trakcie zamykania
            }
            else
            {
                // Błąd przy otwartym połączeniu - może być problemem sieciowym
            }
            // Rozważ zamknięcie połączenia, jeśli błąd jest poważny i stan jest nadal Open
            if (webSocket.State == WebSocketState.Open && ex.WebSocketErrorCode != WebSocketError.Success) // Dodatkowy warunek
            {
                await CloseAndDisposeWebSocketAsync(webSocket, WebSocketCloseStatus.ProtocolError, $"Error sending message: {ex.WebSocketErrorCode}");
            }
        }
        catch (ObjectDisposedException)
        {
            Console.WriteLine("Cannot send message, WebSocket has been disposed.");
        }
        catch (Exception ex)
        {
            // Inny, nieoczekiwany błąd
            Console.WriteLine($"Unexpected error sending message: {ex.GetType().Name} - {ex.Message}");
            await CloseAndDisposeWebSocketAsync(webSocket, WebSocketCloseStatus.InternalServerError, $"Unexpected error sending message");
        }
    }

    // Metoda pomocnicza do wysyłania standardowej odpowiedzi o błędzie
    private async Task SendErrorResponseAsync(WebSocket socket, Guid correlationId, string errorMessage, CancellationToken cancellationToken)
    {
        var errorResponse = new Response
        {
            CorrelationId = correlationId,
            Success = false,
            ErrorMessage = errorMessage
        };
        string xmlErrorResponse = XmlSerializationHelper.Serialize(errorResponse);
        await SendMessageAsync(socket, xmlErrorResponse, cancellationToken);
    }

    // Metoda pomocnicza do bezpiecznego zamykania i zwalniania WebSocket
    private async Task CloseAndDisposeWebSocketAsync(WebSocket? webSocket, WebSocketCloseStatus closeStatus, string statusDescription)
    {
        if (webSocket == null) return;

        Console.WriteLine($"Attempting to close WebSocket: Status={closeStatus}, Desc={statusDescription}, CurrentState={webSocket.State}");

        // Zamknij tylko jeśli stan na to pozwala
        if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
        {
            try
            {
                // Użyj krótkiego timeoutu na zamknięcie
                using var closeCts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                await webSocket.CloseAsync(closeStatus, statusDescription, closeCts.Token);
                Console.WriteLine($"WebSocket closed with status: {closeStatus}");
            }
            catch (OperationCanceledException) { Console.WriteLine("WebSocket close operation timed out."); }
            catch (WebSocketException ex) { Console.WriteLine($"WebSocketException during CloseAsync: {ex.WebSocketErrorCode} - {ex.Message}"); }
            catch (ObjectDisposedException) { Console.WriteLine("WebSocket was already disposed during CloseAsync."); }
            catch (Exception ex) { Console.WriteLine($"Unexpected error during CloseAsync: {ex.GetType().Name} - {ex.Message}"); }
        }
        else
        {
            Console.WriteLine($"WebSocket not in Open or CloseReceived state ({webSocket.State}), skipping CloseAsync.");
        }

        // Zawsze zwolnij zasoby
        try
        {
            webSocket.Dispose();
            Console.WriteLine("WebSocket disposed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error disposing WebSocket: {ex.Message}");
        }
    }

    // Metoda do zatrzymywania serwera
    public async Task StopAsync()
    {
        Console.WriteLine("Stopping WebSocket Server...");
        if (_listener?.IsListening ?? false)
        {
            _listener.Stop(); // Zatrzymaj akceptowanie nowych połączeń
        }

        _cts?.Cancel(); // Anuluj wszystkie aktywne pętle odbioru i oczekiwanie w StartAsync

        // Utwórz listę zadań zamykających dla wszystkich aktywnych klientów
        var closingTasks = new List<Task>();
        var clientsToClose = _clients.ToArray(); // Kopia, aby uniknąć modyfikacji podczas iteracji
        _clients.Clear(); // Wyczyść od razu, aby nowe połączenia (teoretycznie) nie mogły wejść

        foreach (var kvp in clientsToClose)
        {
            Guid clientId = kvp.Key;
            WebSocket socket = kvp.Value;
            Console.WriteLine($"Initiating close for client {clientId}...");
            closingTasks.Add(CloseAndDisposeWebSocketAsync(socket, WebSocketCloseStatus.NormalClosure, "Server shutting down"));
        }

        // Poczekaj na zakończenie zamykania wszystkich klientów (z timeoutem)
        if (closingTasks.Any())
        {
            Console.WriteLine($"Waiting for {closingTasks.Count} clients to close...");
            try
            {
                await Task.WhenAll(closingTasks).WaitAsync(TimeSpan.FromSeconds(5)); // Czekaj max 5 sekund
                Console.WriteLine("All client closing tasks completed or timed out.");
            }
            catch (TimeoutException) { Console.WriteLine("Timeout waiting for clients to close."); }
            catch (Exception ex) { Console.WriteLine($"Error during Task.WhenAll for closing clients: {ex.Message}"); }
        }
        else
        {
            Console.WriteLine("No active clients to close.");
        }

        // Główny token jest już anulowany, StartAsync powinno się zakończyć
        Console.WriteLine("WebSocket Server StopAsync finished.");
    }

    public void Dispose()
    {
        Console.WriteLine("Disposing WebSocket Server...");
        if (!_cts.IsCancellationRequested)
        {
            StopAsync().Wait(TimeSpan.FromSeconds(10)); // Daj czas na zatrzymanie z timeoutem
        }

        _cts?.Dispose(); // Zwolnij zasoby tokenu
        _listener?.Close(); // Zamknij listener (Stop jest wywoływane w StopAsync)

        // Upewnij się, że wszystkie sockety są zwolnione (na wypadek gdyby StopAsync zawiodło)
        foreach (var socket in _clients.Values) { socket.Dispose(); }
        _clients.Clear();

        Console.WriteLine("WebSocket Server disposed.");
        GC.SuppressFinalize(this); // Zapobiegaj finalizacji, jeśli Dispose zostało wywołane
    }
}

public interface IServerLogicRouter
{
    Task<Response> RouteRequestAsync(Request request);
}