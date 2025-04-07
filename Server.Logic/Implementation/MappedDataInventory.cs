using ClientServer.Shared.Data.API;
using ClientServer.Shared.Logic.API;

namespace Server.Logic.Implementation
{
    internal class MappedDataInventory : IInventory
    {
        public Guid Id { get; } = Guid.Empty;
        public int Capacity { get; }
        public List<IItem> Items { get; }

        public MappedDataInventory(IInventoryDataTransferObject inventoryData)
        {
            List<IItem> mappedItems = new List<IItem>();

            foreach (IItemDataTransferObject item in inventoryData.Items)
            {
                mappedItems.Add(new MappedDataItem(item));
            }

            Id = inventoryData.Id;
            Capacity = inventoryData.Capacity;
            Items = mappedItems;
        }
    }
}
