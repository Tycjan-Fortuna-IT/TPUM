using ClientServer.Shared.Data.API;

namespace Server.Data.Tests
{
    internal class DummyInventory : IInventory
    {
        public Guid Id { get; } = Guid.Empty;
        public int Capacity { get; }
        public List<IItem> Items { get; }

        public DummyInventory(int capacity)
        {
            Capacity = capacity;
            Items = new List<IItem>();
        }

        public DummyInventory(Guid id, int capacity)
        {
            Id = id;
            Capacity = capacity;
            Items = new List<IItem>();
        }
    }
}
