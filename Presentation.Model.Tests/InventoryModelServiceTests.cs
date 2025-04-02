using Logic.API;
using Presentation.Model.API;

namespace Presentation.Model.Tests
{
    [TestClass]
    public class InventoryModelServiceTests
    {
        private DummyInventoryLogic _dummyInventoryLogic = null!;
        private DummyItemLogic _dummyItemLogic = null!;

        private IInventoryModelService _inventoryModelService = null!;

        private Guid _inv1Id, _inv2Id;
        private Guid _item1Id, _item2Id;

        private ConcreteInventoryDto _invDto1 = null!;
        private ConcreteInventoryDto _invDto2 = null!;
        private ConcreteItemDto _itemDto1 = null!;
        private ConcreteItemDto _itemDto2 = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            // Instantiate Dummies
            _dummyInventoryLogic = new DummyInventoryLogic();
            _dummyItemLogic = new DummyItemLogic();

            // Create Test DTOs
            _item1Id = Guid.NewGuid();
            _item2Id = Guid.NewGuid();
            _itemDto1 = new ConcreteItemDto { Id = _item1Id, Name = "Potion", Price = 10, MaintenanceCost = 0 };
            _itemDto2 = new ConcreteItemDto { Id = _item2Id, Name = "Scroll", Price = 25, MaintenanceCost = 1 };
            _dummyItemLogic.Items.Add(_itemDto1.Id, _itemDto1);
            _dummyItemLogic.Items.Add(_itemDto2.Id, _itemDto2);

            _inv1Id = Guid.NewGuid();
            _inv2Id = Guid.NewGuid();
            _invDto1 = new ConcreteInventoryDto { Id = _inv1Id, Capacity = 10, Items = new List<IItemDataTransferObject> { _itemDto1, _itemDto2 } };
            _invDto2 = new ConcreteInventoryDto { Id = _inv2Id, Capacity = 5, Items = new List<IItemDataTransferObject>() }; // Empty
            _dummyInventoryLogic.Inventories.Add(_invDto1.Id, _invDto1);
            _dummyInventoryLogic.Inventories.Add(_invDto2.Id, _invDto2);

            _inventoryModelService = ModelFactory.CreateInventoryModelService(_dummyInventoryLogic);
        }

        [TestMethod]
        public void GetAllInventories_WhenCalled_ReturnsAllMappedInventoryModels()
        {
            List<IInventoryModel> inventories = _inventoryModelService.GetAllInventories().ToList();

            Assert.IsNotNull(inventories);
            Assert.AreEqual(2, inventories.Count);

            // Verify mapping for one inventory
            IInventoryModel? inv1Model = inventories.FirstOrDefault(i => i.Id == _inv1Id);
            Assert.IsNotNull(inv1Model);
            Assert.AreEqual(_inv1Id, inv1Model.Id);
            Assert.AreEqual(10, inv1Model.Capacity);
            Assert.IsNotNull(inv1Model.Items);
            Assert.AreEqual(2, inv1Model.Items.Count());

            IItemModel? item1Model = inv1Model.Items.FirstOrDefault(itm => itm.Id == _item1Id);
            Assert.IsNotNull(item1Model);
            Assert.AreEqual("Potion", item1Model.Name);
            Assert.AreEqual(10, item1Model.Price);
        }

        [TestMethod]
        public void GetInventory_ExistingId_ReturnsCorrectMappedInventoryModel()
        {
            IInventoryModel? inventory = _inventoryModelService.GetInventory(_inv1Id);

            Assert.IsNotNull(inventory);
            Assert.AreEqual(_inv1Id, inventory.Id);
            Assert.AreEqual(10, inventory.Capacity);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(2, inventory.Items.Count());
            Assert.AreEqual("Potion", inventory.Items.First().Name);
        }

        [TestMethod]
        public void GetInventory_ExistingEmptyInventory_ReturnsMappedInventoryModelWithEmptyItems()
        {
            IInventoryModel? inventory = _inventoryModelService.GetInventory(_inv2Id);

            Assert.IsNotNull(inventory);
            Assert.AreEqual(_inv2Id, inventory.Id);
            Assert.AreEqual(5, inventory.Capacity);
            Assert.IsNotNull(inventory.Items);
            Assert.AreEqual(0, inventory.Items.Count());
        }

        [TestMethod]
        public void GetInventory_NonExistingId_ReturnsNull()
        {
            Guid nonExistingId = Guid.NewGuid();

            IInventoryModel? inventory = _inventoryModelService.GetInventory(nonExistingId);

            Assert.IsNull(inventory);
        }
    }
}