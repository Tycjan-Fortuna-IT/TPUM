using ClientServer.Shared.Logic.API;
using ClientServer.Shared.WebSocket;
using Server.Logic.API;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Server.Presentation
{
    internal static class Server
    {
        private static IHeroLogic _heroLogic = LogicFactory.CreateHeroLogic();
        private static IInventoryLogic _inventoryLogic = LogicFactory.CreateInventoryLogic();
        private static IItemLogic _itemLogic = LogicFactory.CreateItemLogic();
        private static IOrderLogic _orderLogic = LogicFactory.CreateOrderLogic();

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

        private async static Task SyncItems()
        {
            IEnumerable<IItemDataTransferObject> items = _itemLogic.GetAll();
            List<SerializableItem> itemsToSerialize = items.Select(item => new SerializableItem(item.Id, item.Name, item.Price, item.MaintenanceCost)).ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<SerializableItem>));
            string xml;
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, itemsToSerialize);
                xml = writer.ToString();
            }

            // Send the xml to the client
            await CurrentConnection.SendAsync("ITEMS|" + xml);

            Console.WriteLine("[SERVER]: Sync items");
        }

        private async static Task SyncInventories()
        {
            IEnumerable<IInventoryDataTransferObject> inves = _inventoryLogic.GetAll();
            List<SerializableInventory> inventoriesToSerialize = inves.Select(inv =>
                new SerializableInventory(inv.Id, inv.Capacity,
                    inv.Items.Select(item => new SerializableItem(item.Id, item.Name, item.Price, item.MaintenanceCost)).ToList())
                ).ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<SerializableInventory>));
            string xml;
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, inventoriesToSerialize);
                xml = writer.ToString();
            }

            // Send the xml to the client
            await CurrentConnection.SendAsync("INVENTORIES|" + xml);

            Console.WriteLine("[SERVER]: Sync inventories");
        }

        private async static Task SyncHeroes()
        {
            IEnumerable<IHeroDataTransferObject> heroes = _heroLogic.GetAll();
            List<SerializableHero> heroesToSerialize = heroes.Select(hero =>
                new SerializableHero(hero.Id, hero.Name, hero.Gold,
                    new SerializableInventory(hero.Inventory.Id, hero.Inventory.Capacity,
                    hero.Inventory.Items.Select(item => new SerializableItem(item.Id, item.Name, item.Price, item.MaintenanceCost)).ToList())
                )).ToList();

            XmlSerializer serializer = new XmlSerializer(typeof(List<SerializableHero>));
            string xml;
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, heroesToSerialize);
                xml = writer.ToString();
            }

            // Send the xml to the client
            await CurrentConnection.SendAsync("HEROES|" + xml);

            Console.WriteLine("[SERVER]: Sync heroes");
        }

        private static async Task CreateOrder(string message)
        {
            // Example message: "POST /orders/{id}/buyer/{buyerId}/items/{item1},{item2},..."
            string[] parts = message.Split(new[] { "POST /orders/", "/buyer/", "/items/" }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
                return;

            Guid orderId = Guid.Parse(parts[0]);
            Guid buyerId = Guid.Parse(parts[1]);
            List<Guid> itemIds = parts[2].Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(Guid.Parse)
                                    .ToList();

            await SynchronizeWithClients();
        }

        private static async Task SynchronizeWithClients()
        {
            await SyncItems();
            await SyncHeroes();
            await SyncItems();
        }

        private static async void ParseMessage(string message)
        {
            Console.WriteLine($"[CLIENT]: {message}");

            if (message.Contains("GET /items"))
                await SyncItems();
            else if (message.Contains("GET /heroes"))
                await SyncHeroes();
            else if (message.Contains("GET /inventories"))
                await SyncInventories();
            else if (message.Contains("POST /orders"))
                await CreateOrder(message);
        }

    }
}