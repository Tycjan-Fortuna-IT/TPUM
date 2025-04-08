using ClientServer.Shared.Data.API;
using ClientServer.Shared.Logic.API;
using Client.Logic.API;
using Client.Logic.Implementation;

namespace Client.Logic.Tests
{
    [TestClass]
    public class OrderLogicTests
    {
        private IOrderLogic _logic = default!;

        [TestInitialize]
        public void SetUp()
        {
            _logic = LogicFactory.CreateOrderLogic(new DummyDataRepository());
        }

        private class TestItem : IItem
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Name { get; set; } = "Test Item";
            public int Price { get; set; } = 0;
            public int MaintenanceCost { get; set; } = 0;
        }

        private class TestInventory : IInventory
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public int Capacity { get; set; } = 10;
            public List<IItem> Items { get; set; } = new List<IItem>();
        }

        private class TestHero : IHero
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Name { get; set; } = "Test Hero Buyer";
            public float Gold { get; set; } = 100f;
            public IInventory Inventory { get; set; } = new TestInventory();
        }

        private class TestOrder : IOrder
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public IHero Buyer { get; set; } = new TestHero();
            public IEnumerable<IItem> ItemsToBuy { get; set; } = new List<IItem>();
        }

        [TestMethod]
        public void Map_ShouldCorrectlyMapLocalOrderImplementationToDto()
        {
            var itemToBuy1 = new TestItem { Id = Guid.NewGuid(), Name = "Ordered Sword" };
            var itemToBuy2 = new TestItem { Id = Guid.NewGuid(), Name = "Ordered Shield" };
            var buyerInventoryItem = new TestItem { Id = Guid.NewGuid(), Name = "Buyers Existing Item" };

            var buyerInventory = new TestInventory { Id = Guid.NewGuid(), Capacity = 5 };
            buyerInventory.Items.Add(buyerInventoryItem);

            var testBuyer = new TestHero
            {
                Id = Guid.NewGuid(),
                Name = "Rich Buyer",
                Gold = 10000f,
                Inventory = buyerInventory
            };

            var testOrder = new TestOrder
            {
                Id = Guid.NewGuid(),
                Buyer = testBuyer,
                ItemsToBuy = new List<IItem> { itemToBuy1, itemToBuy2 }
            };

            IOrderDataTransferObject? result = OrderLogic.Map(testOrder);

            Assert.IsNotNull(result);
            Assert.AreEqual(testOrder.Id, result.Id);

            Assert.IsNotNull(result.Buyer);
            Assert.AreEqual(testBuyer.Id, result.Buyer.Id);
            Assert.AreEqual(testBuyer.Name, result.Buyer.Name);
            Assert.AreEqual(testBuyer.Gold, result.Buyer.Gold);

            Assert.IsNotNull(result.Buyer.Inventory);
            Assert.AreEqual(buyerInventory.Id, result.Buyer.Inventory.Id);
            Assert.AreEqual(1, result.Buyer.Inventory.Items.Count());
            Assert.AreEqual(buyerInventoryItem.Id, result.Buyer.Inventory.Items.First().Id);

            Assert.IsNotNull(result.ItemsToBuy);
            Assert.AreEqual(2, result.ItemsToBuy.Count());

            var resultItemToBuy1 = result.ItemsToBuy.FirstOrDefault(i => i.Id == itemToBuy1.Id);
            var resultItemToBuy2 = result.ItemsToBuy.FirstOrDefault(i => i.Id == itemToBuy2.Id);

            Assert.IsNotNull(resultItemToBuy1);
            Assert.AreEqual(itemToBuy1.Name, resultItemToBuy1.Name);

            Assert.IsNotNull(resultItemToBuy2);
            Assert.AreEqual(itemToBuy2.Name, resultItemToBuy2.Name);
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllOrders()
        {
            IHeroDataTransferObject buyer = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            IItemDataTransferObject item1 = new DummyItemDataTransferObject(Guid.NewGuid(), "Item1", 100, 10);
            IItemDataTransferObject item2 = new DummyItemDataTransferObject(Guid.NewGuid(), "Item2", 200, 20);
            IOrderDataTransferObject order1 = new DummyOrderDataTransferObject(Guid.NewGuid(), buyer, new List<IItemDataTransferObject> { item1 });
            IOrderDataTransferObject order2 = new DummyOrderDataTransferObject(Guid.NewGuid(), buyer, new List<IItemDataTransferObject> { item2 });

            _logic.Add(order1);
            _logic.Add(order2);

            IEnumerable<IOrderDataTransferObject> orders = _logic.GetAll();

            Assert.IsNotNull(orders);
            Assert.AreEqual(2, orders.Count());
            Assert.IsTrue(orders.Any(o => o.Id == order1.Id));
            Assert.IsTrue(orders.Any(o => o.Id == order2.Id));
        }

        [TestMethod]
        public void Get_ShouldReturnOrder_WhenOrderExists()
        {
            Guid orderId = Guid.NewGuid();
            IHeroDataTransferObject buyer = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            IItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Item1", 100, 10);
            IOrderDataTransferObject order = new DummyOrderDataTransferObject(orderId, buyer, new List<IItemDataTransferObject> { item });

            _logic.Add(order);

            IOrderDataTransferObject? result = _logic.Get(orderId);

            Assert.IsNotNull(result);
            Assert.AreEqual(orderId, result.Id);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            Guid orderId = Guid.NewGuid();

            IOrderDataTransferObject? result = _logic.Get(orderId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Add_ShouldAddOrderToRepository()
        {
            Guid orderId = Guid.NewGuid();
            IHeroDataTransferObject buyer = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            IItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Item1", 100, 10);
            IOrderDataTransferObject order = new DummyOrderDataTransferObject(orderId, buyer, new List<IItemDataTransferObject> { item });

            _logic.Add(order);

            IOrderDataTransferObject? result = _logic.Get(orderId);
            Assert.IsNotNull(result);
            Assert.AreEqual(orderId, result.Id);
        }

        [TestMethod]
        public void RemoveById_ShouldRemoveOrder_WhenOrderExists()
        {
            Guid orderId = Guid.NewGuid();
            IHeroDataTransferObject buyer = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            IItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Item1", 100, 10);
            IOrderDataTransferObject order = new DummyOrderDataTransferObject(orderId, buyer, new List<IItemDataTransferObject> { item });

            _logic.Add(order);

            bool result = _logic.RemoveById(orderId);

            Assert.IsTrue(result);
            IOrderDataTransferObject? removedOrder = _logic.Get(orderId);
            Assert.IsNull(removedOrder);
        }

        [TestMethod]
        public void RemoveById_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            Guid orderId = Guid.NewGuid();

            bool result = _logic.RemoveById(orderId);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Remove_ShouldRemoveOrder_WhenOrderExists()
        {
            Guid orderId = Guid.NewGuid();
            IHeroDataTransferObject buyer = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            IItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Item1", 100, 10);
            IOrderDataTransferObject order = new DummyOrderDataTransferObject(orderId, buyer, new List<IItemDataTransferObject> { item });

            _logic.Add(order);

            bool result = _logic.Remove(order);

            Assert.IsTrue(result);
            IOrderDataTransferObject? removedOrder = _logic.Get(orderId);
            Assert.IsNull(removedOrder);
        }

        [TestMethod]
        public void Remove_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            IHeroDataTransferObject buyer = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            IItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Item1", 100, 10);
            IOrderDataTransferObject order = new DummyOrderDataTransferObject(Guid.NewGuid(), buyer, new List<IItemDataTransferObject> { item });

            bool result = _logic.Remove(order);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Update_ShouldUpdateOrder_WhenOrderExists()
        {
            Guid orderId = Guid.NewGuid();
            IHeroDataTransferObject buyer = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            IItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Item1", 100, 10);
            IOrderDataTransferObject order = new DummyOrderDataTransferObject(orderId, buyer, new List<IItemDataTransferObject> { item });

            _logic.Add(order);

            IHeroDataTransferObject newBuyer = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero2", 1500, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            IOrderDataTransferObject updatedOrder = new DummyOrderDataTransferObject(orderId, newBuyer, new List<IItemDataTransferObject> { item });

            bool result = _logic.Update(orderId, updatedOrder);

            Assert.IsTrue(result);
            IOrderDataTransferObject? updatedOrderResult = _logic.Get(orderId);
            Assert.AreEqual("Hero2", updatedOrderResult!.Buyer.Name);
        }

        [TestMethod]
        public void Update_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            Guid orderId = Guid.NewGuid();
            IHeroDataTransferObject newBuyer = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero2", 1500, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            IOrderDataTransferObject updatedOrder = new DummyOrderDataTransferObject(orderId, newBuyer, new List<IItemDataTransferObject> { new DummyItemDataTransferObject(Guid.NewGuid(), "Item1", 100, 10) });

            bool result = _logic.Update(orderId, updatedOrder);

            Assert.IsFalse(result);
        }
    }
}
