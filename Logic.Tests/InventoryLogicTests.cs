using Logic.API;

namespace Logic.Tests
{
    [TestClass]
    public class InventoryLogicTests
    {
        private IInventoryLogic _logic = default!;

        [TestInitialize]
        public void SetUp()
        {
            _logic = LogicFactory.CreateInventoryLogic(new DummyDataRepository());
        }

        [TestMethod]
        public void Add_ShouldAddInventoryToRepository()
        {
            DummyInventoryDataTransferObject inventory = new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>());
            _logic.Add(inventory);
            IInventoryDataTransferObject? retrieved = _logic.Get(inventory.Id);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(inventory.Capacity, retrieved.Capacity);
        }

        [TestMethod]
        public void Get_ShouldReturnInventory_WhenExists()
        {
            Guid inventoryId = Guid.NewGuid();
            DummyInventoryDataTransferObject inventory = new DummyInventoryDataTransferObject(inventoryId, 10, new List<IItemDataTransferObject>());
            _logic.Add(inventory);
            IInventoryDataTransferObject? retrieved = _logic.Get(inventoryId);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(inventory.Capacity, retrieved.Capacity);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenInventoryDoesNotExist()
        {
            Guid inventoryId = Guid.NewGuid();
            IInventoryDataTransferObject? result = _logic.Get(inventoryId);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RemoveById_ShouldRemoveInventory_WhenExists()
        {
            Guid inventoryId = Guid.NewGuid();
            DummyInventoryDataTransferObject inventory = new DummyInventoryDataTransferObject(inventoryId, 10, new List<IItemDataTransferObject>());
            _logic.Add(inventory);
            bool removed = _logic.RemoveById(inventoryId);
            Assert.IsTrue(removed);
            Assert.IsNull(_logic.Get(inventoryId));
        }

        [TestMethod]
        public void RemoveById_ShouldReturnFalse_WhenInventoryDoesNotExist()
        {
            Guid inventoryId = Guid.NewGuid();
            bool removed = _logic.RemoveById(inventoryId);
            Assert.IsFalse(removed);
        }

        [TestMethod]
        public void Remove_ShouldRemoveInventory_WhenExists()
        {
            DummyInventoryDataTransferObject inventory = new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>());
            _logic.Add(inventory);
            bool removed = _logic.Remove(inventory);
            Assert.IsTrue(removed);
            Assert.IsNull(_logic.Get(inventory.Id));
        }

        [TestMethod]
        public void Remove_ShouldReturnFalse_WhenInventoryDoesNotExist()
        {
            DummyInventoryDataTransferObject inventory = new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>());
            bool removed = _logic.Remove(inventory);
            Assert.IsFalse(removed);
        }

        [TestMethod]
        public void Update_ShouldUpdateInventory_WhenExists()
        {
            Guid inventoryId = Guid.NewGuid();
            DummyInventoryDataTransferObject inventory = new DummyInventoryDataTransferObject(inventoryId, 10, new List<IItemDataTransferObject>());
            _logic.Add(inventory);
            DummyInventoryDataTransferObject updatedInventory = new DummyInventoryDataTransferObject(inventoryId, 20, new List<IItemDataTransferObject>());
            bool updated = _logic.Update(inventoryId, updatedInventory);
            Assert.IsTrue(updated);
            IInventoryDataTransferObject? retrieved = _logic.Get(inventoryId);
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(updatedInventory.Capacity, retrieved.Capacity);
        }

        [TestMethod]
        public void Update_ShouldReturnFalse_WhenInventoryDoesNotExist()
        {
            DummyInventoryDataTransferObject inventory = new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>());
            bool updated = _logic.Update(inventory.Id, inventory);
            Assert.IsFalse(updated);
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllInventories()
        {
            _logic.Add(new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            _logic.Add(new DummyInventoryDataTransferObject(Guid.NewGuid(), 15, new List<IItemDataTransferObject>()));
            IEnumerable<IInventoryDataTransferObject> inventories = _logic.GetAll();
            Assert.AreEqual(2, inventories.Count());
        }
    }
}
