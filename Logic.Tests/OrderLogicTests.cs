using ClientServer.Shared.Logic.API;
using Server.Logic.API;

namespace Server.Logic.Tests
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
