using ClientServer.Shared.Data.API;
using ClientServer.Shared.Logic.API;

namespace Server.Logic.Implementation
{
    internal class MappedDataItem : IItem
    {
        public Guid Id { get; } = Guid.Empty;
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        public MappedDataItem(IItemDataTransferObject itemData)
        {
            Id = itemData.Id;
            Name = itemData.Name;
            Price = itemData.Price;
            MaintenanceCost = itemData.MaintenanceCost;
        }
    }
}
