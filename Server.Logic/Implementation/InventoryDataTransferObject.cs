using ClientServer.Shared.Logic.API;

namespace Server.Logic.Implementation
{
    internal class InventoryDataTransferObject : IInventoryDataTransferObject
    {
        public Guid Id { get; }
        public int Capacity { get; }
        public IEnumerable<IItemDataTransferObject> Items { get; }

        public InventoryDataTransferObject(Guid id, int capacity, IEnumerable<IItemDataTransferObject> items)
        {
            Id = id;
            Capacity = capacity;
            Items = items;
        }
    }
}
