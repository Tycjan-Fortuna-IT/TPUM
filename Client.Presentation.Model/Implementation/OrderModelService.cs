﻿using Client.Presentation.Model.API;
using ClientServer.Shared.Logic.API;

namespace Client.Presentation.Model.Implementation
{
    internal class OrderModelService : IOrderModelService
    {
        private readonly IOrderLogic _orderLogic;
        private readonly IHeroLogic _heroLogic;
        private readonly IItemLogic _itemLogic;

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
            IOrderDataTransferObject? dto = _orderLogic.Get(id);
            return dto == null ? null : new OrderModel(dto); // Map DTO to Model
        }

        public void AddOrder(Guid id, Guid buyerId, IEnumerable<Guid> itemIds)
        {
            IHeroDataTransferObject buyerDto = _heroLogic.Get(buyerId)!;
            List<IItemDataTransferObject> itemDtos = new List<IItemDataTransferObject>();
            foreach (Guid itemId in itemIds)
            {
                IItemDataTransferObject? itemDto = _itemLogic.Get(itemId);
                if (itemDto == null)
                {
                    throw new InvalidOperationException($"Item with ID {itemId} not found.");
                }
                itemDtos.Add(itemDto);
            }

            TransientOrderDTO transientDto = new TransientOrderDTO(id, buyerDto, itemDtos);
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
    }
}