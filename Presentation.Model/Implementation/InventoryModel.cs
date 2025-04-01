using Presentation.Model.API;
using Logic.API;
using System;
using System.Collections.Generic;

namespace Presentation.Model.Implementation
{
    internal class InventoryModel : IInventoryModel
    {
        public Guid Id { get; }
        public int Capacity { get; }
        public IEnumerable<IItemModel> Items { get; }

        // DTO
        public InventoryModel(IInventoryDataTransferObject dto)
        {
            Id = dto.Id;
            Capacity = dto.Capacity;
            Items = dto.Items?.Select(itemDto => new ItemModel(itemDto)).ToList() ?? Enumerable.Empty<IItemModel>();
        }

        // direct creation
        public InventoryModel(Guid id, int capacity, IEnumerable<IItemModel> items)
        {
            Id = id;
            Capacity = capacity;
            Items = items ?? Enumerable.Empty<IItemModel>();
        }
    }
}