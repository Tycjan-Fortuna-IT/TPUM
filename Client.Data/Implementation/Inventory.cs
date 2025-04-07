using ClientServer.Shared.Data.API;

namespace Client.Data.Implementation
{
    internal class Inventory : IInventory
    {
        public Guid Id { get; } = Guid.Empty;
        public int Capacity { get; }
        public List<IItem> Items { get; }

        public Inventory(int capacity)
        {
            Capacity = capacity;
            Items = new List<IItem>();
        }

        public Inventory(Guid id, int capacity)
        {
            Id = id;
            Capacity = capacity;
            Items = new List<IItem>();
        }

        public Inventory(Guid id, int capacity, List<IItem> items)
        {
            Id = id;
            Capacity = capacity;
            Items = items;
        }
    }
}
