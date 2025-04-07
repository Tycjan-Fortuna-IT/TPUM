//using Presentation.Model.Implementation;
using Client.Presentation.Model.API;
using Logic.API;


namespace Presentation.Model.Tests
{
    [TestClass]
    public class OrderModelServiceTests
    {
        private DummyOrderLogic _dummyOrderLogic = null!;
        private DummyHeroLogic _dummyHeroLogic = null!;
        private DummyItemLogic _dummyItemLogic = null!;
        private DummyInventoryLogic _dummyInventoryLogic = null!;

        private IOrderModelService _orderModelService = null!;

        private Guid _order1Id;
        private Guid _hero1Id, _hero2Id;
        private Guid _inv1Id, _inv2Id;
        private Guid _item1Id, _item2Id, _item3Id;

        private DummyOrderDto _orderDto1 = null!;
        private DummyHeroDto _heroDto1 = null!;
        private DummyHeroDto _heroDto2 = null!;
        private DummyInventoryDto _invDto1 = null!;
        private DummyInventoryDto _invDto2 = null!;
        private DummyItemDto _itemDto1 = null!;
        private DummyItemDto _itemDto2 = null!;
        private DummyItemDto _itemDto3 = null!; // Item not in order

        [TestInitialize]
        public void TestInitialize()
        {
            _dummyOrderLogic = new DummyOrderLogic();
            _dummyHeroLogic = new DummyHeroLogic();
            _dummyItemLogic = new DummyItemLogic();
            _dummyInventoryLogic = new DummyInventoryLogic();

            _item1Id = Guid.NewGuid();
            _item2Id = Guid.NewGuid();
            _item3Id = Guid.NewGuid();
            _itemDto1 = new DummyItemDto { Id = _item1Id, Name = "Axe", Price = 120, MaintenanceCost = 6 };
            _itemDto2 = new DummyItemDto { Id = _item2Id, Name = "Helmet", Price = 75, MaintenanceCost = 2 };
            _itemDto3 = new DummyItemDto { Id = _item3Id, Name = "Boots", Price = 40, MaintenanceCost = 1 };
            _dummyItemLogic.Items.Add(_itemDto1.Id, _itemDto1);
            _dummyItemLogic.Items.Add(_itemDto2.Id, _itemDto2);
            _dummyItemLogic.Items.Add(_itemDto3.Id, _itemDto3);


            _inv1Id = Guid.NewGuid();
            _inv2Id = Guid.NewGuid();
            _invDto1 = new DummyInventoryDto { Id = _inv1Id, Capacity = 10, Items = new List<IItemDataTransferObject>() };
            _invDto2 = new DummyInventoryDto { Id = _inv2Id, Capacity = 5, Items = new List<IItemDataTransferObject>() };
            _dummyInventoryLogic.Inventories.Add(_invDto1.Id, _invDto1);
            _dummyInventoryLogic.Inventories.Add(_invDto2.Id, _invDto2);

            _hero1Id = Guid.NewGuid();
            _hero2Id = Guid.NewGuid();
            _heroDto1 = new DummyHeroDto { Id = _hero1Id, Name = "BuyerHero", Gold = 1000f, Inventory = _invDto1 };
            _heroDto2 = new DummyHeroDto { Id = _hero2Id, Name = "OtherHero", Gold = 500f, Inventory = _invDto2 };
            _dummyHeroLogic.Heroes.Add(_heroDto1.Id, _heroDto1);
            _dummyHeroLogic.Heroes.Add(_heroDto2.Id, _heroDto2);

            _order1Id = Guid.NewGuid();
            _orderDto1 = new DummyOrderDto
            {
                Id = _order1Id,
                Buyer = _heroDto1,
                ItemsToBuy = new List<IItemDataTransferObject> { _itemDto1, _itemDto2 }
            };
            _dummyOrderLogic.Orders.Add(_orderDto1.Id, _orderDto1);


            _orderModelService = ModelFactory.CreateOrderModelService(_dummyOrderLogic, _dummyHeroLogic, _dummyItemLogic);
        }

        [TestMethod]
        public void GetAllOrders_WhenCalled_ReturnsAllMappedOrderModels()
        {
            List<IOrderModel> orders = _orderModelService.GetAllOrders().ToList();

            Assert.IsNotNull(orders);
            Assert.AreEqual(1, orders.Count);

            // Verify mapping for the order
            IOrderModel order1Model = orders.First();
            Assert.AreEqual(_order1Id, order1Model.Id);

            // Verify Buyer mapping
            Assert.IsNotNull(order1Model.Buyer);
            Assert.AreEqual(_hero1Id, order1Model.Buyer.Id);
            Assert.AreEqual("BuyerHero", order1Model.Buyer.Name);

            // Verify ItemsToBuy mapping
            Assert.IsNotNull(order1Model.ItemsToBuy);
            Assert.AreEqual(2, order1Model.ItemsToBuy.Count());

            IItemModel? item1Model = order1Model.ItemsToBuy.FirstOrDefault(i => i.Id == _item1Id);
            IItemModel? item2Model = order1Model.ItemsToBuy.FirstOrDefault(i => i.Id == _item2Id);
            Assert.IsNotNull(item1Model);
            Assert.IsNotNull(item2Model);
            Assert.AreEqual("Axe", item1Model.Name);
            Assert.AreEqual("Helmet", item2Model.Name);
        }

        [TestMethod]
        public void GetOrder_ExistingId_ReturnsCorrectMappedOrderModel()
        {
            IOrderModel? order = _orderModelService.GetOrder(_order1Id);

            Assert.IsNotNull(order);
            Assert.AreEqual(_order1Id, order.Id);

            // Verify Buyer
            Assert.IsNotNull(order.Buyer);
            Assert.AreEqual(_hero1Id, order.Buyer.Id);

            // Verify Items
            Assert.IsNotNull(order.ItemsToBuy);
            Assert.AreEqual(2, order.ItemsToBuy.Count());
            Assert.IsTrue(order.ItemsToBuy.Any(i => i.Id == _item1Id));
            Assert.IsTrue(order.ItemsToBuy.Any(i => i.Id == _item2Id));
        }

        [TestMethod]
        public void GetOrder_NonExistingId_ReturnsNull()
        {
            Guid nonExistingId = Guid.NewGuid();

            IOrderModel? order = _orderModelService.GetOrder(nonExistingId);

            Assert.IsNull(order);
        }

        [TestMethod]
        public void AddOrder_ValidData_CallsLogicAddWithCorrectDto()
        {
            Guid newId = Guid.NewGuid();
            Guid buyerId = _hero2Id; // Different hero buys
            List<Guid> itemIdsToBuy = new List<Guid> { _item3Id }; // Buy boots

            _orderModelService.AddOrder(newId, buyerId, itemIdsToBuy);

            Assert.IsTrue(_dummyOrderLogic.Orders.ContainsKey(newId));
            IOrderDataTransferObject addedDto = _dummyOrderLogic.Orders[newId];
            Assert.AreEqual(newId, addedDto.Id);

            // Check if Buyer DTO and Item DTOs correctly assigned
            Assert.IsNotNull(addedDto.Buyer);
            Assert.AreEqual(buyerId, addedDto.Buyer.Id);

            Assert.IsNotNull(addedDto.ItemsToBuy);
            Assert.AreEqual(1, addedDto.ItemsToBuy.Count());
            Assert.AreEqual(_item3Id, addedDto.ItemsToBuy.First().Id);
        }

        [TestMethod]
        public void RemoveOrder_ExistingId_CallsLogicRemoveAndReturnsTrue()
        {
            Guid targetId = _order1Id;
            Assert.IsTrue(_dummyOrderLogic.Orders.ContainsKey(targetId));

            bool result = _orderModelService.RemoveOrder(targetId);

            Assert.IsTrue(result);
            Assert.IsFalse(_dummyOrderLogic.Orders.ContainsKey(targetId));
        }

        [TestMethod]
        public void RemoveOrder_NonExistingId_CallsLogicRemoveAndReturnsFalse()
        {
            Guid nonExistingId = Guid.NewGuid();
            int initialCount = _dummyOrderLogic.Orders.Count;

            bool result = _orderModelService.RemoveOrder(nonExistingId);

            Assert.IsFalse(result);
            Assert.AreEqual(initialCount, _dummyOrderLogic.Orders.Count);
        }

        [TestMethod]
        public void TriggerPeriodicOrderProcessing_WhenCalled_CallsLogicMethod()
        {
            int initialCallCount = _dummyOrderLogic.PeriodicProcessingCallCount;

            _orderModelService.TriggerPeriodicOrderProcessing();

            Assert.AreEqual(initialCallCount + 1, _dummyOrderLogic.PeriodicProcessingCallCount);
        }

    }

}
