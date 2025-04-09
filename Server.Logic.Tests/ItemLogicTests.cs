using ClientServer.Shared.Data.API;
using ClientServer.Shared.Logic.API;
using Server.Logic.API;

namespace Server.Logic.Tests
{
    [TestClass]
    public class ItemLogicTests
    {
        private IItemLogic _logic = default!;

        [TestInitialize]
        public void SetUp()
        {
            _logic = LogicFactory.CreateItemLogic(new DummyDataRepository());
        }

        private class TestItem : IItem
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Name { get; set; } = "Default Test Item";
            public int Price { get; set; } = 10;
            public int MaintenanceCost { get; set; } = 1;
        }

        //[TestMethod]
        //public void Map_ShouldCorrectlyMapLocalItemImplementationToDto()
        //{
        //    var testItem = new TestItem
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = "Magic Dust",
        //        Price = 25,
        //        MaintenanceCost = 0
        //    };

        //    IItemDataTransferObject? result = ItemLogic.Map(testItem);

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(testItem.Id, result.Id);
        //    Assert.AreEqual(testItem.Name, result.Name);
        //    Assert.AreEqual(testItem.Price, result.Price);
        //    Assert.AreEqual(testItem.MaintenanceCost, result.MaintenanceCost);
        //}

        [TestMethod]
        public void Add_ShouldAddItemToLogic()
        {
            DummyItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Sword", 100, 5);
            _logic.Add(item);

            IItemDataTransferObject? result = _logic.Get(item.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(item.Name, result.Name);
        }

        [TestMethod]
        public void Get_ShouldReturnItem_WhenItemExists()
        {
            DummyItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Shield", 150, 8);
            _logic.Add(item);

            IItemDataTransferObject? result = _logic.Get(item.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(item.Price, result.Price);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenItemDoesNotExist()
        {
            IItemDataTransferObject? result = _logic.Get(Guid.NewGuid());
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RemoveById_ShouldRemoveItem_WhenItemExists()
        {
            DummyItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Bow", 120, 6);
            _logic.Add(item);
            bool removed = _logic.RemoveById(item.Id);

            Assert.IsTrue(removed);
            Assert.IsNull(_logic.Get(item.Id));
        }

        [TestMethod]
        public void RemoveById_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            bool removed = _logic.RemoveById(Guid.NewGuid());
            Assert.IsFalse(removed);
        }

        [TestMethod]
        public void Remove_ShouldRemoveItem_WhenItemExists()
        {
            DummyItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Dagger", 50, 2);
            _logic.Add(item);
            bool removed = _logic.Remove(item);

            Assert.IsTrue(removed);
            Assert.IsNull(_logic.Get(item.Id));
        }

        [TestMethod]
        public void Remove_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            DummyItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Axe", 200, 10);
            bool removed = _logic.Remove(item);
            Assert.IsFalse(removed);
        }

        [TestMethod]
        public void Update_ShouldUpdateItem_WhenItemExists()
        {
            DummyItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Spear", 180, 9);
            _logic.Add(item);
            DummyItemDataTransferObject updatedItem = new DummyItemDataTransferObject(item.Id, "Updated Spear", 200, 12);
            bool updated = _logic.Update(item.Id, updatedItem);

            Assert.IsTrue(updated);
            IItemDataTransferObject? result = _logic.Get(item.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Spear", result.Name);
        }

        [TestMethod]
        public void Update_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            DummyItemDataTransferObject item = new DummyItemDataTransferObject(Guid.NewGuid(), "Hammer", 220, 15);
            bool updated = _logic.Update(item.Id, item);
            Assert.IsFalse(updated);
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllItems()
        {
            DummyItemDataTransferObject item1 = new DummyItemDataTransferObject(Guid.NewGuid(), "Bow", 120, 6);
            DummyItemDataTransferObject item2 = new DummyItemDataTransferObject(Guid.NewGuid(), "Crossbow", 140, 7);
            _logic.Add(item1);
            _logic.Add(item2);

            IEnumerable<IItemDataTransferObject> items = _logic.GetAll();
            Assert.AreEqual(2, items.Count());
        }
    }
}