using Presentation.Model.API;
using System;
using System.Collections.Generic;
using System.Linq; // Needed for Select

namespace Presentation.Model.Implementation
{
    internal class InventoryModel : IInventoryModel
    {
        public Guid Id { get; }
        public int Capacity { get; }
        public IEnumerable<IItemModel> Items { get; }

        // Constructor taking Logic DTO for mapping
        public InventoryModel(Logic.API.IInventoryDataTransferObject dto)
        {
            Id = dto.Id;
            Capacity = dto.Capacity;
            // Map each Item DTO to an ItemModel
            Items = dto.Items?.Select(itemDto => new ItemModel(itemDto)).ToList() ?? new List<IItemModel>();
        }

        // Constructor for direct creation if needed
        public InventoryModel(Guid id, int capacity, IEnumerable<IItemModel> items)
        {
            Id = id;
            Capacity = capacity;
            Items = items?.ToList() ?? new List<IItemModel>();
        }
    }
}