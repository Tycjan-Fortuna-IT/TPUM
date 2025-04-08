using Server.Data.API;
using ClientServer.Shared.Data.API;

namespace Server.Data.Tests
{
    [TestClass]
    public class InventoryDataRepositoryTests
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
        public void Add_ShouldAddInventoryToRepository()
        {
            DummyInventory inventory = new DummyInventory(10);

            _repository.AddInventory(inventory);

            Assert.IsTrue(_context.Inventories.ContainsKey(inventory.Id));
            Assert.AreEqual(inventory.Capacity, _context.Inventories[inventory.Id].Capacity);
        }

        [TestMethod]
        public void Get_ShouldReturnInventory_WhenInventoryExists()
        {
            Guid inventoryId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(inventoryId, 10);
            _context.Inventories[inventoryId] = inventory;

            IInventory? result = _repository.GetInventory(inventoryId);

            Assert.IsNotNull(result);
            Assert.AreEqual(inventory.Capacity, result.Capacity);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenInventoryDoesNotExist()
        {
            Guid inventoryId = Guid.NewGuid();

            IInventory? result = _repository.GetInventory(inventoryId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Remove_ShouldRemoveInventory_WhenInventoryExists()
        {
            Guid inventoryId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(inventoryId, 10);
            _context.Inventories[inventoryId] = inventory;

            bool result = _repository.RemoveInventory(inventory);

            Assert.IsTrue(result);
            Assert.IsFalse(_context.Inventories.ContainsKey(inventoryId));
        }

        [TestMethod]
        public void Remove_ShouldReturnFalse_WhenInventoryDoesNotExist()
        {
            DummyInventory inventory = new DummyInventory(Guid.NewGuid(), 10);

            bool result = _repository.RemoveInventory(inventory);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RemoveById_ShouldRemoveInventory_WhenInventoryExists()
        {
            Guid inventoryId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(inventoryId, 10);
            _context.Inventories[inventoryId] = inventory;

            bool result = _repository.RemoveInventoryById(inventoryId);

            Assert.IsTrue(result);
            Assert.IsFalse(_context.Inventories.ContainsKey(inventoryId));
        }

        [TestMethod]
        public void RemoveById_ShouldReturnFalse_WhenInventoryDoesNotExist()
        {
            Guid inventoryId = Guid.NewGuid();

            bool result = _repository.RemoveInventoryById(inventoryId);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Update_ShouldUpdateInventory_WhenInventoryExists()
        {
            Guid inventoryId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(inventoryId, 10);
            _context.Inventories[inventoryId] = inventory;

            DummyInventory updatedInventory = new DummyInventory(inventoryId, 15);

            bool result = _repository.UpdateInventory(inventoryId, updatedInventory);

            Assert.IsTrue(result);
            Assert.AreEqual(updatedInventory.Capacity, _context.Inventories[inventoryId].Capacity);
        }

        [TestMethod]
        public void Update_ShouldReturnFalse_WhenInventoryDoesNotExist()
        {
            Guid inventoryId = Guid.NewGuid();
            DummyInventory updatedInventory = new DummyInventory(inventoryId, 15);

            bool result = _repository.UpdateInventory(inventoryId, updatedInventory);

            Assert.IsFalse(result);
        }
    }
}
