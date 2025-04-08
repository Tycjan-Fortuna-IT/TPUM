using Server.Data.API;
using ClientServer.Shared.Data.API;

namespace Server.Data.Tests
{
    [TestClass]
    public class OrderDataRepositoryTests
    {
        private IDataContext _context = default!;
        private IDataRepository _repository = default!;

        [TestInitialize]
        public void SetUp()
        {
            _context = DataContextFactory.CreateDataContext();
            _repository = DataRepositoryFactory.CreateDataRepository(_context);
        }

        [TestMethod]
        public void Add_ShouldAddOrderToRepository()
        {
            DummyHero hero = new DummyHero("Hero1", 1000, new DummyInventory(10));
            List<IItem> itemsToBuy = new List<IItem> { new DummyItem("Sword", 100, 5) };
            DummyOrder order = new DummyOrder(hero, itemsToBuy);

            _repository.AddOrder(order);

            Assert.IsTrue(_context.Orders.ContainsKey(order.Id));
            Assert.AreEqual(hero.Name, _context.Orders[order.Id].Buyer.Name);
        }

        [TestMethod]
        public void Get_ShouldReturnOrder_WhenOrderExists()
        {
            Guid orderId = Guid.NewGuid();
            DummyHero hero = new DummyHero("Hero1", 1000, new DummyInventory(10));
            List<IItem> itemsToBuy = new List<IItem> { new DummyItem("Sword", 100, 5) };
            DummyOrder order = new DummyOrder(orderId, hero, itemsToBuy);
            _context.Orders[orderId] = order;

            IOrder? result = _repository.GetOrder(orderId);

            Assert.IsNotNull(result);
            Assert.AreEqual(orderId, result.Id);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            Guid orderId = Guid.NewGuid();

            IOrder? result = _repository.GetOrder(orderId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Remove_ShouldRemoveOrder_WhenOrderExists()
        {
            Guid orderId = Guid.NewGuid();
            DummyHero hero = new DummyHero("Hero1", 1000, new DummyInventory(10));
            List<IItem> itemsToBuy = new List<IItem> { new DummyItem("Sword", 100, 5) };
            DummyOrder order = new DummyOrder(orderId, hero, itemsToBuy);
            _context.Orders[orderId] = order;

            bool result = _repository.RemoveOrder(order);

            Assert.IsTrue(result);
            Assert.IsFalse(_context.Orders.ContainsKey(orderId));
        }

        [TestMethod]
        public void Remove_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            DummyHero hero = new DummyHero("Hero1", 1000, new DummyInventory(10));
            List<IItem> itemsToBuy = new List<IItem> { new DummyItem("Sword", 100, 5) };
            DummyOrder order = new DummyOrder(Guid.NewGuid(), hero, itemsToBuy);

            bool result = _repository.RemoveOrder(order);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RemoveById_ShouldRemoveOrder_WhenOrderExists()
        {
            Guid orderId = Guid.NewGuid();
            DummyHero hero = new DummyHero("Hero1", 1000, new DummyInventory(10));
            List<IItem> itemsToBuy = new List<IItem> { new DummyItem("Sword", 100, 5) };
            DummyOrder order = new DummyOrder(orderId, hero, itemsToBuy);
            _context.Orders[orderId] = order;

            bool result = _repository.RemoveOrderById(orderId);

            Assert.IsTrue(result);
            Assert.IsFalse(_context.Orders.ContainsKey(orderId));
        }

        [TestMethod]
        public void RemoveById_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            Guid orderId = Guid.NewGuid();

            bool result = _repository.RemoveOrderById(orderId);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Update_ShouldUpdateOrder_WhenOrderExists()
        {
            Guid orderId = Guid.NewGuid();
            DummyHero hero = new DummyHero("Hero1", 1000, new DummyInventory(10));
            List<IItem> itemsToBuy = new List<IItem> { new DummyItem("Sword", 100, 5) };
            DummyOrder order = new DummyOrder(orderId, hero, itemsToBuy);
            _context.Orders[orderId] = order;

            List<IItem> updatedItems = new List<IItem> { new DummyItem("Axe", 150, 8) };
            DummyOrder updatedOrder = new DummyOrder(orderId, hero, updatedItems);

            bool result = _repository.UpdateOrder(orderId, updatedOrder);

            Assert.IsTrue(result);
            Assert.AreEqual(updatedItems.Count, _context.Orders[orderId].ItemsToBuy.Count());
        }

        [TestMethod]
        public void Update_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            Guid orderId = Guid.NewGuid();
            DummyHero hero = new DummyHero("Hero1", 1000, new DummyInventory(10));
            List<IItem> updatedItems = new List<IItem> { new DummyItem("Axe", 150, 8) };
            DummyOrder updatedOrder = new DummyOrder(orderId, hero, updatedItems);

            bool result = _repository.UpdateOrder(orderId, updatedOrder);

            Assert.IsFalse(result);
        }
    }
}
