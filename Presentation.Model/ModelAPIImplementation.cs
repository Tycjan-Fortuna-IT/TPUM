using Presentation.Model.API;
using Presentation.Model.Implementation;
using Logic.API;

namespace Presentation.Model
{
    public class ModelAPIImplementation : ModelAbstractAPI
    {
        private readonly IHeroLogic _heroLogic;
        private readonly IInventoryLogic _inventoryLogic;
        private readonly IItemLogic _itemLogic;
        private readonly IOrderLogic _orderLogic;
        private readonly IDTOMapper _mapper;

        public ModelAPIImplementation(
            IHeroLogic heroLogic,
            IInventoryLogic inventoryLogic,
            IItemLogic itemLogic,
            IOrderLogic orderLogic,
            IDTOMapper mapper)
        {
            _heroLogic = heroLogic;
            _inventoryLogic = inventoryLogic;
            _itemLogic = itemLogic;
            _orderLogic = orderLogic;
            _mapper = mapper;
        }

        // Hero-related operations
        public override IEnumerable<IHeroModel> GetAllHeroes()
            => _heroLogic.GetAll().Select(_mapper.MapToHeroModel);

        public override IHeroModel? GetHero(Guid id)
        {
            var dto = _heroLogic.Get(id);
            return dto != null ? _mapper.MapToHeroModel(dto) : null;
        }

        public override void AddHero(IHeroModel hero)
            => _heroLogic.Add(_mapper.MapToHeroDTO(hero));

        public override bool RemoveHero(Guid id)
            => _heroLogic.RemoveById(id);

        public override bool UpdateHero(Guid id, IHeroModel hero)
            => _heroLogic.Update(id, _mapper.MapToHeroDTO(hero));

        public override void PerformHeroMaintenance()
            => _heroLogic.PeriodicItemMaintenanceDeduction();

        // Inventory-related operations
        public override IEnumerable<IInventoryModel> GetAllInventories()
            => _inventoryLogic.GetAll().Select(_mapper.MapToInventoryModel);

        public override IInventoryModel? GetInventory(Guid id)
        {
            var dto = _inventoryLogic.Get(id);
            return dto != null ? _mapper.MapToInventoryModel(dto) : null;
        }

        public override void AddInventory(IInventoryModel inventory)
            => _inventoryLogic.Add(_mapper.MapToInventoryDTO(inventory));

        public override bool RemoveInventory(Guid id)
            => _inventoryLogic.RemoveById(id);

        public override bool UpdateInventory(Guid id, IInventoryModel inventory)
            => _inventoryLogic.Update(id, _mapper.MapToInventoryDTO(inventory));

        // Item-related operations
        public override IEnumerable<IItemModel> GetAllItems()
            => _itemLogic.GetAll().Select(_mapper.MapToItemModel);

        public override IItemModel? GetItem(Guid id)
        {
            var dto = _itemLogic.Get(id);
            return dto != null ? _mapper.MapToItemModel(dto) : null;
        }

        public override void AddItem(IItemModel item)
            => _itemLogic.Add(_mapper.MapToItemDTO(item));

        public override bool RemoveItem(Guid id)
            => _itemLogic.RemoveById(id);

        public override bool UpdateItem(Guid id, IItemModel item)
            => _itemLogic.Update(id, _mapper.MapToItemDTO(item));

        // Order-related operations
        public override IEnumerable<IOrderModel> GetAllOrders()
            => _orderLogic.GetAll().Select(_mapper.MapToOrderModel);

        public override IOrderModel? GetOrder(Guid id)
        {
            var dto = _orderLogic.Get(id);
            return dto != null ? _mapper.MapToOrderModel(dto) : null;
        }

        public override void AddOrder(IOrderModel order)
            => _orderLogic.Add(_mapper.MapToOrderDTO(order));

        public override bool RemoveOrder(Guid id)
            => _orderLogic.RemoveById(id);

        public override bool UpdateOrder(Guid id, IOrderModel order)
            => _orderLogic.Update(id, _mapper.MapToOrderDTO(order));

        public override void ProcessOrders()
            => _orderLogic.PeriodicOrderProcessing();

        public override void RestockItems()
            => _orderLogic.RestockItems();
    }
}
