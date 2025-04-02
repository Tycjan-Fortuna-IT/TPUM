using Logic.API;
using Presentation.Model.API;

namespace Presentation.Model.Tests
{
    [TestClass]
    public class ItemModelServiceTests
    {
        private DummyItemLogic _dummyItemLogic = null!;
        private IItemModelService _itemModelService = null!;

        private Guid _item1Id, _item2Id;

        private ConcreteItemDto _itemDto1 = null!;
        private ConcreteItemDto _itemDto2 = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _dummyItemLogic = new DummyItemLogic();

            _item1Id = Guid.NewGuid();
            _item2Id = Guid.NewGuid();
            _itemDto1 = new ConcreteItemDto { Id = _item1Id, Name = "Elixir", Price = 50, MaintenanceCost = 2 };
            _itemDto2 = new ConcreteItemDto { Id = _item2Id, Name = "Dagger", Price = 30, MaintenanceCost = 1 };
            _dummyItemLogic.Items.Add(_itemDto1.Id, _itemDto1);
            _dummyItemLogic.Items.Add(_itemDto2.Id, _itemDto2);

            _itemModelService = ModelFactory.CreateItemModelService(_dummyItemLogic);
        }

        [TestMethod]
        public void GetAllItems_WhenCalled_ReturnsAllMappedItemModels()
        {
            List<IItemModel> items = _itemModelService.GetAllItems().ToList();

            Assert.IsNotNull(items);
            Assert.AreEqual(2, items.Count);

            // Verify mapping for one item
            IItemModel? item1Model = items.FirstOrDefault(i => i.Id == _item1Id);
            Assert.IsNotNull(item1Model);
            Assert.AreEqual(_item1Id, item1Model.Id);
            Assert.AreEqual("Elixir", item1Model.Name);
            Assert.AreEqual(50, item1Model.Price);
            Assert.AreEqual(2, item1Model.MaintenanceCost);
        }

        [TestMethod]
        public void GetItem_ExistingId_ReturnsCorrectMappedItemModel()
        {
            IItemModel? item = _itemModelService.GetItem(_item1Id);

            Assert.IsNotNull(item);
            Assert.AreEqual(_item1Id, item.Id);
            Assert.AreEqual("Elixir", item.Name);
            Assert.AreEqual(50, item.Price);
            Assert.AreEqual(2, item.MaintenanceCost);
        }

        [TestMethod]
        public void GetItem_NonExistingId_ReturnsNull()
        {
            Guid nonExistingId = Guid.NewGuid();

            IItemModel? item = _itemModelService.GetItem(nonExistingId);

            Assert.IsNull(item);
        }

        [TestMethod]
        public void AddItem_ValidData_CallsLogicAdd()
        {
            Guid newId = Guid.NewGuid();
            string newName = "Rope";
            int newPrice = 5;
            int newCost = 0;

            _itemModelService.AddItem(newId, newName, newPrice, newCost);

            Assert.IsTrue(_dummyItemLogic.Items.ContainsKey(newId));
            IItemDataTransferObject addedDto = _dummyItemLogic.Items[newId];
            Assert.AreEqual(newName, addedDto.Name);
            Assert.AreEqual(newPrice, addedDto.Price);
            Assert.AreEqual(newCost, addedDto.MaintenanceCost);
        }

        [TestMethod]
        public void RemoveItem_ExistingId_CallsLogicRemoveAndReturnsTrue()
        {
            Guid targetId = _item1Id;
            Assert.IsTrue(_dummyItemLogic.Items.ContainsKey(targetId));

            bool result = _itemModelService.RemoveItem(targetId);

            Assert.IsTrue(result);
            Assert.IsFalse(_dummyItemLogic.Items.ContainsKey(targetId));
        }

        [TestMethod]
        public void RemoveItem_NonExistingId_CallsLogicRemoveAndReturnsFalse()
        {
            Guid nonExistingId = Guid.NewGuid();
            int initialCount = _dummyItemLogic.Items.Count;

            bool result = _itemModelService.RemoveItem(nonExistingId);

            Assert.IsFalse(result);
            Assert.AreEqual(initialCount, _dummyItemLogic.Items.Count);
        }

        [TestMethod]
        public void UpdateItem_ExistingId_CallsLogicUpdateAndReturnsTrue()
        {
            Guid targetId = _item1Id;
            string updatedName = "Greater Elixir";
            int updatedPrice = 100;
            int updatedCost = 4;

            bool result = _itemModelService.UpdateItem(targetId, updatedName, updatedPrice, updatedCost);

            Assert.IsTrue(result);
            Assert.IsTrue(_dummyItemLogic.Items.ContainsKey(targetId));
            IItemDataTransferObject updatedDto = _dummyItemLogic.Items[targetId];
            Assert.AreEqual(updatedName, updatedDto.Name);
            Assert.AreEqual(updatedPrice, updatedDto.Price);
            Assert.AreEqual(updatedCost, updatedDto.MaintenanceCost);
        }

        [TestMethod]
        public void UpdateItem_NonExistingId_CallsLogicUpdateAndReturnsFalse()
        {
            Guid nonExistingId = Guid.NewGuid();
            int initialCount = _dummyItemLogic.Items.Count;

            bool result = _itemModelService.UpdateItem(nonExistingId, "Fake Item", 1, 0);

            Assert.IsFalse(result);
            Assert.AreEqual(initialCount, _dummyItemLogic.Items.Count);
        }
    }
}