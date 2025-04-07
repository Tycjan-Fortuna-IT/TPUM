using Client.Data.Websocket;
using ClientServer.Shared.Data.API;
using Server.Presentation;
using System.Xml.Serialization;

namespace Client.Data.Implementation
{
    internal class DataContext : IDataContext
    {
        private readonly Dictionary<Guid, IHero> _heroes = new Dictionary<Guid, IHero>();
        private readonly Dictionary<Guid, IItem> _items = new Dictionary<Guid, IItem>();
        private readonly Dictionary<Guid, IInventory> _inventories = new Dictionary<Guid, IInventory>();
        private readonly Dictionary<Guid, IOrder> _orders = new Dictionary<Guid, IOrder>();

        public Dictionary<Guid, IHero> Heroes => _heroes;
        public Dictionary<Guid, IItem> Items => _items;
        public Dictionary<Guid, IInventory> Inventories => _inventories;
        public Dictionary<Guid, IOrder> Orders => _orders;

        public event Action OnDataChanged = delegate { };

        public DataContext()
        {
            WebSocketClient.OnMessage += WebSocketClient_OnMessage;
        }

        private void WebSocketClient_OnMessage(string obj)
        {
            Console.WriteLine("[SERVER]");
            // await CurrentConnection.SendAsync("ITEMS|" + xml);
            Console.WriteLine(obj);
            switch (obj.Split('|')[0])
            {
                case "ITEMS":
                    SyncItems(obj.Split('|')[1]);
                    break;
                default:
                    Console.WriteLine("Unknown message type");
                    break;
            }
        }

        private void SyncItems(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<SerializableItem>));
            using (StringReader reader = new StringReader(xml))
            {
                _items.Clear();

                List<SerializableItem> items = (List<SerializableItem>)serializer.Deserialize(reader)!;
                foreach (var item in items)
                {
                    _items[item.Id] = new Item(item.Id, item.Name, item.Price, item.MaintenanceCost);
                }
            }

            //WebSocketConnection.onDataArrived.Invoke();
        }
    }
}