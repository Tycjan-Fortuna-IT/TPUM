using Presentation.Model.API;
using System;

namespace Presentation.Model.Implementation
{
    internal class ItemModel : IItemModel
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Price { get; }
        public int MaintenanceCost { get; }

        // Constructor taking Logic DTO for mapping
        public ItemModel(Logic.API.IItemDataTransferObject dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Price = dto.Price;
            MaintenanceCost = dto.MaintenanceCost;
        }

        // Constructor for direct creation if needed
        public ItemModel(Guid id, string name, int price, int maintenanceCost)
        {
            Id = id;
            Name = name;
            Price = price;
            MaintenanceCost = maintenanceCost;
        }
    }
}
