using Presentation.Model.API;
using Logic.API;
using Presentation.Model.Implementation.Mapper;

namespace Presentation.Model.Implementation
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
            var dto = _orderLogic.Get(id);
            return dto == null ? null : new OrderModel(dto); // Map DTO to Model
        }

        public void AddOrder(Guid id, Guid buyerId, IEnumerable<Guid> itemIds)
        {
            var buyerDto = _heroLogic.Get(buyerId);
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