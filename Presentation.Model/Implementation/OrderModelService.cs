using Presentation.Model.API;
using Logic.API;
using System;
using System.Collections.Generic;
using System.Linq;
using Presentation.Model.Implementation.Transient;

namespace Presentation.Model.Implementation
{
    internal class OrderModelService : IOrderModelService
    {
        private readonly IOrderLogic _orderLogic;
        private readonly IHeroLogic _heroLogic;   // Needed to get Buyer DTO for Add
        private readonly IItemLogic _itemLogic;   // Needed to get Item DTOs for Add

        public OrderModelService(IOrderLogic orderLogic, IHeroLogic heroLogic, IItemLogic itemLogic)
        {
            _orderLogic = orderLogic ?? throw new ArgumentNullException(nameof(orderLogic));
            _heroLogic = heroLogic ?? throw new ArgumentNullException(nameof(heroLogic));
            _itemLogic = itemLogic ?? throw new ArgumentNullException(nameof(itemLogic));
        }

        public IEnumerable<IOrderModel> GetAllOrders()
        {
            return _orderLogic.GetAll()
                             .Select(dto => new OrderModel(dto)); // Map DTO to Model
        }

        public IOrderModel? GetOrder(Guid id)
        {
            var dto = _orderLogic.Get(id);
            return dto == null ? null : new OrderModel(dto); // Map DTO to Model
        }

        public void AddOrder(Guid id, Guid buyerId, IEnumerable<Guid> itemIds)
        {
            // Fetch the required DTOs from the Logic layer
            var buyerDto = _heroLogic.Get(buyerId);
            if (buyerDto == null)
            {
                throw new InvalidOperationException($"Buyer Hero with ID {buyerId} not found.");
            }

            var itemDtos = new List<IItemDataTransferObject>();
            foreach (var itemId in itemIds)
            {
                var itemDto = _itemLogic.Get(itemId);
                if (itemDto == null)
                {
                    throw new InvalidOperationException($"Item with ID {itemId} not found.");
                }
                itemDtos.Add(itemDto);
            }

            var transientDto = new TransientOrderDTO(id, buyerDto, itemDtos);
            _orderLogic.Add(transientDto);
        }

        public bool RemoveOrder(Guid id)
        {
            return _orderLogic.RemoveById(id);
        }

        public void TriggerPeriodicOrderProcessing()
        {
            _orderLogic.PeriodicOrderProcessing();
        }

        public void TriggerRestockItems()
        {
            _orderLogic.RestockItems();
        }
    }
}