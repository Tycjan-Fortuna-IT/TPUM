using Presentation.Model.API;
using Logic.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Model.Implementation
{
    internal class OrderModel : IOrderModel
    {
        public Guid Id { get; }
        public IHeroModel Buyer { get; }
        public IEnumerable<IItemModel> ItemsToBuy { get; }

        // DTO
        public OrderModel(IOrderDataTransferObject dto)
        {
            Id = dto.Id;
            Buyer = new HeroModel(dto.Buyer);
            ItemsToBuy = dto.ItemsToBuy?.Select(itemDto => new ItemModel(itemDto)).ToList() ?? Enumerable.Empty<IItemModel>();
        }

        // direct creation
        public OrderModel(Guid id, IHeroModel buyer, IEnumerable<IItemModel> itemsToBuy)
        {
            Id = id;
            Buyer = buyer;
            ItemsToBuy = itemsToBuy?.ToList() ?? new List<IItemModel>();
        }
    }
}
