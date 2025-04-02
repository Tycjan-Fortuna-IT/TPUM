using Logic.API;
using Presentation.Model.API;

namespace Presentation.Model.Tests
{
    [TestClass]
    public class HeroModelServiceTests
    {
        private DummyHeroLogic _dummyHeroLogic = null!;
        private DummyInventoryLogic _dummyInventoryLogic = null!;
        private DummyItemLogic _dummyItemLogic = null!;

        private IHeroModelService _heroModelService = null!;

        private Guid _hero1Id, _hero2Id;
        private Guid _inv1Id, _inv2Id;
        private Guid _item1Id, _item2Id;

        private ConcreteHeroDto _heroDto1 = null!;
        private ConcreteHeroDto _heroDto2 = null!;
        private ConcreteInventoryDto _invDto1 = null!;
        private ConcreteInventoryDto _invDto2 = null!;
        private ConcreteItemDto _itemDto1 = null!;
        private ConcreteItemDto _itemDto2 = null!;


        [TestInitialize]
        public void TestInitialize()
        {
            _dummyHeroLogic = new DummyHeroLogic();
            _dummyInventoryLogic = new DummyInventoryLogic();
            _dummyItemLogic = new DummyItemLogic();

            // Create Test DTOs
            _item1Id = Guid.NewGuid();
            _item2Id = Guid.NewGuid();
            _itemDto1 = new ConcreteItemDto { Id = _item1Id, Name = "Sword", Price = 100, MaintenanceCost = 5 };
            _itemDto2 = new ConcreteItemDto { Id = _item2Id, Name = "Shield", Price = 80, MaintenanceCost = 3 };
            _dummyItemLogic.Items.Add(_itemDto1.Id, _itemDto1);
            _dummyItemLogic.Items.Add(_itemDto2.Id, _itemDto2);


            _inv1Id = Guid.NewGuid();
            _inv2Id = Guid.NewGuid();
            _invDto1 = new ConcreteInventoryDto { Id = _inv1Id, Capacity = 10, Items = new List<IItemDataTransferObject> { _itemDto1 } };
            _invDto2 = new ConcreteInventoryDto { Id = _inv2Id, Capacity = 5, Items = new List<IItemDataTransferObject>() }; // Empty inventory
            _dummyInventoryLogic.Inventories.Add(_invDto1.Id, _invDto1);
            _dummyInventoryLogic.Inventories.Add(_invDto2.Id, _invDto2);


            _hero1Id = Guid.NewGuid();
            _hero2Id = Guid.NewGuid();
            _heroDto1 = new ConcreteHeroDto { Id = _hero1Id, Name = "Bob", Gold = 500f, Inventory = _invDto1 };
            _heroDto2 = new ConcreteHeroDto { Id = _hero2Id, Name = "Achilles", Gold = 300f, Inventory = _invDto2 };
            _dummyHeroLogic.Heroes.Add(_heroDto1.Id, _heroDto1);
            _dummyHeroLogic.Heroes.Add(_heroDto2.Id, _heroDto2);


            _heroModelService = ModelFactory.CreateHeroModelService(_dummyHeroLogic, _dummyInventoryLogic);
        }

        [TestMethod]
        public void GetAllHeroes_WhenCalled_ReturnsAllMappedHeroModels()
        {
            List<IHeroModel> heroes = _heroModelService.GetAllHeroes().ToList();

            Assert.IsNotNull(heroes);
            Assert.AreEqual(2, heroes.Count);

            IHeroModel? BobModel = heroes.FirstOrDefault(h => h.Id == _hero1Id);
            Assert.IsNotNull(BobModel);
            Assert.AreEqual("Bob", BobModel.Name);
            Assert.AreEqual(500f, BobModel.Gold);
            Assert.IsNotNull(BobModel.Inventory);
            Assert.AreEqual(_inv1Id, BobModel.Inventory.Id);
            Assert.AreEqual(10, BobModel.Inventory.Capacity);
            Assert.AreEqual(1, BobModel.Inventory.Items.Count());
            Assert.AreEqual(_item1Id, BobModel.Inventory.Items.First().Id);
            Assert.AreEqual("Sword", BobModel.Inventory.Items.First().Name);
        }

        [TestMethod]
        public void GetHero_ExistingId_ReturnsCorrectMappedHeroModel()
        {
            IHeroModel? hero = _heroModelService.GetHero(_hero1Id);

            Assert.IsNotNull(hero);
            Assert.AreEqual(_hero1Id, hero.Id);
            Assert.AreEqual("Bob", hero.Name);
            Assert.AreEqual(500f, hero.Gold);
            Assert.IsNotNull(hero.Inventory);
            Assert.AreEqual(_inv1Id, hero.Inventory.Id);
            Assert.AreEqual(1, hero.Inventory.Items.Count());
        }

        [TestMethod]
        public void GetHero_NonExistingId_ReturnsNull()
        {
            Guid nonExistingId = Guid.NewGuid();

            IHeroModel? hero = _heroModelService.GetHero(nonExistingId);

            Assert.IsNull(hero);
        }

        [TestMethod]
        public void AddHero_ValidData_CallsLogicAdd()
        {
            Guid newId = Guid.NewGuid();
            string newName = "Newbie";
            float newGold = 10f;
            Guid newInventoryId = _inv2Id;

            _heroModelService.AddHero(newId, newName, newGold, newInventoryId);

            Assert.IsTrue(_dummyHeroLogic.Heroes.ContainsKey(newId));
            IHeroDataTransferObject addedDto = _dummyHeroLogic.Heroes[newId];
            Assert.AreEqual(newName, addedDto.Name);
            Assert.AreEqual(newGold, addedDto.Gold);
            Assert.AreEqual(newInventoryId, addedDto.Inventory.Id);
        }

        [TestMethod]
        public void RemoveHero_ExistingId_CallsLogicRemoveAndReturnsTrue()
        {
            Guid targetId = _hero1Id;
            Assert.IsTrue(_dummyHeroLogic.Heroes.ContainsKey(targetId));

            bool result = _heroModelService.RemoveHero(targetId);

            Assert.IsTrue(result);
            Assert.IsFalse(_dummyHeroLogic.Heroes.ContainsKey(targetId));
        }

        [TestMethod]
        public void RemoveHero_NonExistingId_CallsLogicRemoveAndReturnsFalse()
        {
            Guid nonExistingId = Guid.NewGuid();
            int initialCount = _dummyHeroLogic.Heroes.Count;

            bool result = _heroModelService.RemoveHero(nonExistingId);

            Assert.IsFalse(result);
            Assert.AreEqual(initialCount, _dummyHeroLogic.Heroes.Count);
        }


        [TestMethod]
        public void UpdateHero_ExistingId_CallsLogicUpdateAndReturnsTrue()
        {
            Guid targetId = _hero1Id;
            string updatedName = "Bob the Updated";
            float updatedGold = 550f;
            Guid updatedInventoryId = _inv2Id; // Change inventory

            // Find the inventory DTO to pass to the updated hero DTO
            _dummyInventoryLogic.Inventories.TryGetValue(updatedInventoryId, out IInventoryDataTransferObject? updatedInventoryDto);
            Assert.IsNotNull(updatedInventoryDto);

            bool result = _heroModelService.UpdateHero(targetId, updatedName, updatedGold, updatedInventoryId);

            Assert.IsTrue(result);
            Assert.IsTrue(_dummyHeroLogic.Heroes.ContainsKey(targetId));
            IHeroDataTransferObject updatedDto = _dummyHeroLogic.Heroes[targetId];
            Assert.AreEqual(updatedName, updatedDto.Name);
            Assert.AreEqual(updatedGold, updatedDto.Gold);
            Assert.IsNotNull(updatedDto.Inventory);
            Assert.AreEqual(updatedInventoryId, updatedDto.Inventory.Id);
        }

        [TestMethod]
        public void UpdateHero_NonExistingId_CallsLogicUpdateAndReturnsFalse()
        {
            Guid nonExistingId = Guid.NewGuid();
            int initialCount = _dummyHeroLogic.Heroes.Count;

            // Pass a valid inventory ID even though the hero doesn't exist
            bool result = _heroModelService.UpdateHero(nonExistingId, "Doesn't Exist", 0f, _inv1Id);

            Assert.IsFalse(result);
            Assert.AreEqual(initialCount, _dummyHeroLogic.Heroes.Count);
        }

        [TestMethod]
        public void TriggerPeriodicItemMaintenanceDeduction_WhenCalled_CallsLogicMethod()
        {
            int initialCallCount = _dummyHeroLogic.PeriodicDeductionCallCount;

            _heroModelService.TriggerPeriodicItemMaintenanceDeduction();

            Assert.AreEqual(initialCallCount + 1, _dummyHeroLogic.PeriodicDeductionCallCount);
        }
    }
}