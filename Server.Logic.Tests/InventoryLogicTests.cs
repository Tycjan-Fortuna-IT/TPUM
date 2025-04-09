using ClientServer.Shared.Data.API;
using ClientServer.Shared.Logic.API;
using Server.Logic.API;

namespace Server.Logic.Tests
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

        private class TestItem : IItem
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Name { get; set; } = "ItemInInventory";
            public int Price { get; set; } = 5;
            public int MaintenanceCost { get; set; } = 0;
        }

        private class TestInventory : IInventory
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public int Capacity { get; set; } = 10;
            public List<IItem> Items { get; set; } = new List<IItem>();
        }

        //public void Map_ShouldCorrectlyMapLocalInventoryImplementationToDto()
        //{
        //    var item1 = new TestItem { Id = Guid.NewGuid(), Name = "Potion" };
        //    var item2 = new TestItem { Id = Guid.NewGuid(), Name = "Scroll" };
        //    var testInventory = new TestInventory
        //    {
        //        Id = Guid.NewGuid(),
        //        Capacity = 5
        //    };
        //    testInventory.Items.Add(item1);
        //    testInventory.Items.Add(item2);

        //    IInventoryDataTransferObject? result = InventoryLogic.Map(testInventory);

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(testInventory.Id, result.Id);
        //    Assert.AreEqual(testInventory.Capacity, result.Capacity);
        //    Assert.IsNotNull(result.Items);
        //    Assert.AreEqual(2, result.Items.Count());

        //    var resultItem1 = result.Items.FirstOrDefault(i => i.Id == item1.Id);
        //    var resultItem2 = result.Items.FirstOrDefault(i => i.Id == item2.Id);

        //    Assert.IsNotNull(resultItem1);
        //    Assert.AreEqual(item1.Name, resultItem1.Name);

        //    Assert.IsNotNull(resultItem2);
        //    Assert.AreEqual(item2.Name, resultItem2.Name);
        //}

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
