using Presentation.Model.API;
using System;
using System.Collections.Generic;
using System.Linq; // Needed for Select

namespace Presentation.Model.Implementation
{
    internal class OrderModel : IOrderModel
    {
        public Guid Id { get; }
        public IHeroModel Buyer { get; }
        public IEnumerable<IItemModel> ItemsToBuy { get; }

        // Constructor taking Logic DTO for mapping
        public OrderModel(Logic.API.IOrderDataTransferObject dto)
        {
            Id = dto.Id;
            // Map the nested DTOs
            Buyer = new HeroModel(dto.Buyer);
            //ItemsToBuy = new List<IItemModel>();
            ItemsToBuy = dto.ItemsToBuy?.Select(itemDto => new ItemModel(itemDto)).ToList() ?? new List<IItemModel>();
        }

        // Constructor for direct creation if needed
        public OrderModel(Guid id, IHeroModel buyer, IEnumerable<IItemModel> itemsToBuy)
        {
            Id = id;
            Buyer = buyer;
            ItemsToBuy = itemsToBuy?.ToList() ?? new List<IItemModel>();
        }
    }
}
