using Shared.Data.API;

namespace Server.Data.Implementation
{
    internal class Inventory : IInventory
    {
        public Guid Id { get; } = Guid.Empty;
        public int Capacity { get; }
        public IEnumerable<IItem> Items { get; }

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
    }
}
