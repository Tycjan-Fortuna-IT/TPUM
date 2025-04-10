﻿using Server.Data.API;
using ClientServer.Shared.Data.API;

namespace Server.Data.Tests
{
    [TestClass]
    public class ItemDataRepositoryTests : DataRepositoryTestBase
    {
        [TestMethod]
        public void Add_ShouldAddItemToRepository()
        {
            DummyItem item = new DummyItem("Sword", 100, 5);

            _repository.AddItem(item);

            Assert.IsTrue(_mockContext.Items.ContainsKey(item.Id));
            Assert.AreEqual(item.Name, _mockContext.Items[item.Id].Name);
        }

        [TestMethod]
        public void Get_ShouldReturnItem_WhenItemExists()
        {
            Guid itemId = Guid.NewGuid();
            DummyItem item = new DummyItem(itemId, "Sword", 100, 5);
            _mockContext.Items[itemId] = item;

            IItem? result = _repository.GetItem(itemId);

            Assert.IsNotNull(result);
            Assert.AreEqual(item.Name, result.Name);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenItemDoesNotExist()
        {
            Guid itemId = Guid.NewGuid();

            IItem? result = _repository.GetItem(itemId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Remove_ShouldRemoveItem_WhenItemExists()
        {
            Guid itemId = Guid.NewGuid();
            DummyItem item = new DummyItem(itemId, "Sword", 100, 5);
            _mockContext.Items[itemId] = item;

            bool result = _repository.RemoveItem(item);

            Assert.IsTrue(result);
            Assert.IsFalse(_mockContext.Items.ContainsKey(itemId));
        }

        [TestMethod]
        public void Remove_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            DummyItem item = new DummyItem(Guid.NewGuid(), "Sword", 100, 5);

            bool result = _repository.RemoveItem(item);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RemoveById_ShouldRemoveItem_WhenItemExists()
        {
            Guid itemId = Guid.NewGuid();
            DummyItem item = new DummyItem(itemId, "Sword", 100, 5);
            _mockContext.Items[itemId] = item;

            bool result = _repository.RemoveItemById(itemId);

            Assert.IsTrue(result);
            Assert.IsFalse(_mockContext.Items.ContainsKey(itemId));
        }

        [TestMethod]
        public void RemoveById_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            Guid itemId = Guid.NewGuid();

            bool result = _repository.RemoveItemById(itemId);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Update_ShouldUpdateItem_WhenItemExists()
        {
            Guid itemId = Guid.NewGuid();
            DummyItem item = new DummyItem(itemId, "Sword", 100, 5);
            _mockContext.Items[itemId] = item;

            DummyItem updatedItem = new DummyItem(itemId, "UpdatedSword", 150, 10);

            bool result = _repository.UpdateItem(itemId, updatedItem);

            Assert.IsTrue(result);
            Assert.AreEqual(updatedItem.Name, _mockContext.Items[itemId].Name);
        }

        [TestMethod]
        public void Update_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            Guid itemId = Guid.NewGuid();
            DummyItem updatedItem = new DummyItem(itemId, "UpdatedSword", 150, 10);

            bool result = _repository.UpdateItem(itemId, updatedItem);

            Assert.IsFalse(result);
        }
    }
}
