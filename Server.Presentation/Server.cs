﻿using ClientServer.Shared.Logic.API;
using ClientServer.Shared.WebSocket;
using Server.Logic.API;
using System.Globalization;
using System.Xml.Serialization;

namespace Server.Presentation
{
    internal static class Server
    {
        private static IHeroLogic _heroLogic = LogicFactory.CreateHeroLogic();
        private static IInventoryLogic _inventoryLogic = LogicFactory.CreateInventoryLogic();
        private static IItemLogic _itemLogic = LogicFactory.CreateItemLogic();
        private static IOrderLogic _orderLogic = LogicFactory.CreateOrderLogic();

        private static IMaintenanceTracker _maintenanceTracker = MaintenanceFactory.CreateTracker();
        private static IMaintenanceReporter _maintenanceReporter = MaintenanceFactory.CreateReporter();

        private static WebSocketConnection CurrentConnection = null!;

        private static Timer? _maintenanceTimer = null;
        private static TimeSpan _interval = TimeSpan.FromSeconds(5);

        private static async Task Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            _maintenanceReporter.Subscribe(_maintenanceTracker, () => { }, (Exception e) => { }, (IHeroDataTransferObject next) =>
            {
                _heroLogic.DeduceMaintenanceCost(next);
            });

            Console.WriteLine($"Starting maintenance timer with interval {_interval.TotalSeconds}s.");
            _maintenanceTimer = new Timer(
                MaintenanceTick,
                null,
                _interval,
                _interval
            );

            Console.WriteLine("[Server]: Starting WebSocket server on port 9081...");
            await WebSocketServer.Server(9081, ConnectionHandler);
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

            IHeroDataTransferObject? buyerDto = _heroLogic.Get(buyerId);
            if (buyerDto == null)
                return;

            List<IItemDataTransferObject> itemDtos = new List<IItemDataTransferObject>();
            foreach (Guid itemId in itemIds)
            {
                IItemDataTransferObject? itemDto = _itemLogic.Get(itemId);
                if (itemDto == null)
                    return;

                itemDtos.Add(itemDto);
            }

            TransientOrderDTO transientDto = new TransientOrderDTO(orderId, buyerDto, itemDtos);
            _orderLogic.Add(transientDto);

            _orderLogic.PeriodicOrderProcessing(); // process the order immediately

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

        private static void MaintenanceTick(object? state)
        {
            foreach (IHeroDataTransferObject hero in _heroLogic.GetAll())
            {
                _maintenanceTracker.Track(hero);
            }

            Task.Run(async () => { await SynchronizeWithClients(); });
        }
    }

    // Item DTO
    internal class TransientItemDTO : IItemDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        public TransientItemDTO(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
    }

    // Inventory DTO
    internal class TransientInventoryDTO : IInventoryDataTransferObject
    {
        public Guid Id { get; }
        public int Capacity { get; }
        public IEnumerable<IItemDataTransferObject> Items { get; }

        public TransientInventoryDTO(Guid id, int capacity, IEnumerable<IItemDataTransferObject> items)
        {
            Id = id;
            Capacity = capacity;
            Items = items?.ToList() ?? new List<IItemDataTransferObject>();
        }
    }

    // Hero DTO
    internal class TransientHeroDTO : IHeroDataTransferObject
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public float Gold { get; set; }
        public IInventoryDataTransferObject Inventory { get; set; }

        public TransientHeroDTO(Guid id, string name, float gold, IInventoryDataTransferObject inventory)
        {
            Id = id;
            Name = name;
            Gold = gold;
            Inventory = inventory;
        }
    }

    // Order DTO
    internal class TransientOrderDTO : IOrderDataTransferObject
    {
        public Guid Id { get; }
        public IHeroDataTransferObject Buyer { get; }
        public IEnumerable<IItemDataTransferObject> ItemsToBuy { get; }

        public TransientOrderDTO(Guid id, IHeroDataTransferObject buyer, IEnumerable<IItemDataTransferObject> itemsToBuy)
        {
            Id = id;
            Buyer = buyer;
            ItemsToBuy = itemsToBuy?.ToList() ?? new List<IItemDataTransferObject>();
        }
    }
}