using ClientServer.Shared.Logic.API;
using Server.Logic.API;

namespace Server.Logic.Tests
{
    [TestClass]
    public class HeroLogicTests
    {
        private IHeroLogic _logic = default!;

        [TestInitialize]
        public void SetUp()
        {
            _logic = LogicFactory.CreateHeroLogic(new DummyDataRepository());
        }

        [TestMethod]
        public void GetAll_ShouldReturnAllHeroes()
        {
            IHeroDataTransferObject hero1 = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            IHeroDataTransferObject hero2 = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero2", 1500, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));

            _logic.Add(hero1);
            _logic.Add(hero2);

            IEnumerable<IHeroDataTransferObject> heroes = _logic.GetAll();

            Assert.IsNotNull(heroes);
            Assert.AreEqual(2, heroes.Count());
            Assert.IsTrue(heroes.Any(h => h.Name == "Hero1"));
            Assert.IsTrue(heroes.Any(h => h.Name == "Hero2"));
        }

        [TestMethod]
        public void Get_ShouldReturnHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            IHeroDataTransferObject hero = new DummyHeroDataTransferObject(heroId, "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            _logic.Add(hero);

            IHeroDataTransferObject? result = _logic.Get(heroId);

            Assert.IsNotNull(result);
            Assert.AreEqual(heroId, result.Id);
            Assert.AreEqual("Hero1", result.Name);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenHeroDoesNotExist()
        {
            Guid heroId = Guid.NewGuid();

            IHeroDataTransferObject? result = _logic.Get(heroId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Add_ShouldAddHeroToRepository()
        {
            IHeroDataTransferObject hero = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));

            _logic.Add(hero);

            IHeroDataTransferObject? result = _logic.Get(hero.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(hero.Name, result.Name);
        }

        [TestMethod]
        public void RemoveById_ShouldRemoveHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            IHeroDataTransferObject hero = new DummyHeroDataTransferObject(heroId, "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            _logic.Add(hero);

            bool result = _logic.RemoveById(heroId);

            Assert.IsTrue(result);
            IHeroDataTransferObject? removedHero = _logic.Get(heroId);
            Assert.IsNull(removedHero);
        }

        [TestMethod]
        public void RemoveById_ShouldReturnFalse_WhenHeroDoesNotExist()
        {
            Guid heroId = Guid.NewGuid();

            bool result = _logic.RemoveById(heroId);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Remove_ShouldRemoveHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            IHeroDataTransferObject hero = new DummyHeroDataTransferObject(heroId, "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            _logic.Add(hero);

            bool result = _logic.Remove(hero);

            Assert.IsTrue(result);
            IHeroDataTransferObject? removedHero = _logic.Get(heroId);
            Assert.IsNull(removedHero);
        }

        [TestMethod]
        public void Remove_ShouldReturnFalse_WhenHeroDoesNotExist()
        {
            IHeroDataTransferObject hero = new DummyHeroDataTransferObject(Guid.NewGuid(), "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));

            bool result = _logic.Remove(hero);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Update_ShouldUpdateHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            IHeroDataTransferObject hero = new DummyHeroDataTransferObject(heroId, "Hero1", 1000, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));
            _logic.Add(hero);

            IHeroDataTransferObject updatedHero = new DummyHeroDataTransferObject(heroId, "UpdatedHero", 1500, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));

            bool result = _logic.Update(heroId, updatedHero);

            Assert.IsTrue(result);
            IHeroDataTransferObject? updatedHeroResult = _logic.Get(heroId);
            Assert.AreEqual("UpdatedHero", updatedHeroResult!.Name);
        }

        [TestMethod]
        public void Update_ShouldReturnFalse_WhenHeroDoesNotExist()
        {
            Guid heroId = Guid.NewGuid();
            IHeroDataTransferObject updatedHero = new DummyHeroDataTransferObject(heroId, "UpdatedHero", 1500, new DummyInventoryDataTransferObject(Guid.NewGuid(), 10, new List<IItemDataTransferObject>()));

            bool result = _logic.Update(heroId, updatedHero);

            Assert.IsFalse(result);
        }
    }
}
