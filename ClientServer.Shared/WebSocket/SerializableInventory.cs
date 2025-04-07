using Server.Presentation;

namespace ClientServer.Shared.WebSocket
{
    [Serializable]
    public class SerializableInventory
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public List<SerializableItem> Items { get; set; }

        public SerializableInventory()
        {
            Id = Guid.NewGuid();
            Capacity = 0;
            Items = new List<SerializableItem>();
        }

        public SerializableInventory(Guid id, int capacity, List<SerializableItem> items)
        {
            Id = id;
            Capacity = capacity;
            Items = items;
        }
    }
}
