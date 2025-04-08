using Client.Logic.API;
using Client.Logic.Implementation;
using Client.Data.Websocket;

namespace Client.Logic.Tests
{
    [TestClass]
    public class ConnectionServiceTests
    {
        private ClientConnectionService _connectionService = null!;
        private List<string> _logMessages = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _connectionService = new ClientConnectionService();
            _logMessages = new List<string>();
            _connectionService.ConnectionLogger = (msg) => _logMessages.Add(msg);
        }

        [TestMethod]
        public async Task Connect_InvalidUriOrConnectionFails_ShouldLogAndReturnFalse()
        {
            Uri testUri = new Uri("ws://invalid-host-that-deffinitely-does-not-exist:9191");
            bool result = true;

            // Act
            try
            {
                result = await _connectionService.Connect(testUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connect test caught: {ex.Message}");
            }

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task FetchItems_NoConnection_ShouldLogNoConnection()
        {
            if (WebSocketClient.CurrentConnection != null)
            {
                await WebSocketClient.Disconnect();
            }

            await _connectionService.FetchItems();

            Assert.IsTrue(_logMessages.Any(m => m.Contains("No connection to server.")));
        }

        [TestMethod]
        public async Task FetchInventories_NoConnection_ShouldLogNoConnection()
        {
            if (WebSocketClient.CurrentConnection != null)
            {
                await WebSocketClient.Disconnect();
            }
            await _connectionService.FetchInventories();
            Assert.IsTrue(_logMessages.Any(m => m.Contains("No connection to server.")));
        }

        [TestMethod]
        public async Task FetchHeroes_NoConnection_ShouldLogNoConnection()
        {
            if (WebSocketClient.CurrentConnection != null)
            {
                await   WebSocketClient.Disconnect();
            }
            await _connectionService.FetchHeroes();
            Assert.IsTrue(_logMessages.Any(m => m.Contains("No connection to server.")));
        }


        [TestCleanup]
        public async Task TestCleanup()
        {
            try
            {
                if (WebSocketClient.CurrentConnection != null)
                {
                    await WebSocketClient.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup error: {ex.Message}");
            }
        }
    }
}