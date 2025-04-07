using Client.Presentation.Model.API;
using Shared.Logic.API;

namespace Client.Presentation.Model.Implementation
{
    internal class ItemModel : IItemModel
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        // DTO
        public ItemModel(IItemDataTransferObject dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Price = dto.Price;
            MaintenanceCost = dto.MaintenanceCost;
        }

        // direct creation
        public ItemModel(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
    }
}
