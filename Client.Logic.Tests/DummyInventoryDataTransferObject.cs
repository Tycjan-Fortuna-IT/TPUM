using ClientServer.Shared.Logic.API;

namespace Client.Logic.Tests
{
    internal class DummyInventoryDataTransferObject : IInventoryDataTransferObject
    {
        public Guid Id { get; }
        public int Capacity { get; }
        public IEnumerable<IItemDataTransferObject> Items { get; }

        public DummyInventoryDataTransferObject(Guid id, int capacity, IEnumerable<IItemDataTransferObject> items)
        {
            Id = id;
            Capacity = capacity;
            Items = items;
        }
    }
}
