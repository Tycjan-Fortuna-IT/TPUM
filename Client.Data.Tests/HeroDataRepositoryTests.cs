using ClientServer.Shared.Data.API;

namespace Client.Data.Tests
{
    [TestClass]
    public class HeroDataRepositoryTests : DataRepositoryTestBase // Dziedziczymy
    {
        [TestMethod]
        public void Add_ShouldAddHeroToRepository()
        {
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero("Hero1", 1000, inventory);

            _repository.AddHero(hero);

            Assert.IsTrue(_mockContext.Heroes.ContainsKey(hero.Id));
            Assert.AreEqual(hero.Name, _mockContext.Heroes[hero.Id].Name);
        }

        [TestMethod]
        public void Get_ShouldReturnHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero(heroId, "Hero1", 1000, inventory);

            _mockContext.Heroes[heroId] = hero;

            IHero? result = _repository.GetHero(heroId);

            Assert.IsNotNull(result);
            Assert.AreEqual(hero.Name, result.Name);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenHeroDoesNotExist()
        {
            Guid heroId = Guid.NewGuid();

            IHero? result = _repository.GetHero(heroId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Remove_ShouldRemoveHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero(heroId, "Hero1", 1000, inventory);
            _mockContext.Heroes[heroId] = hero;

            bool result = _repository.RemoveHero(hero);

            Assert.IsTrue(result);
            Assert.IsFalse(_mockContext.Heroes.ContainsKey(heroId));
        }

        [TestMethod]
        public void Remove_ShouldReturnFalse_WhenHeroDoesNotExist()
        {
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero(Guid.NewGuid(), "Hero1", 1000, inventory);

            bool result = _repository.RemoveHero(hero);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RemoveById_ShouldRemoveHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero(heroId, "Hero1", 1000, inventory);
            _mockContext.Heroes[heroId] = hero;

            bool result = _repository.RemoveHeroById(heroId);

            Assert.IsTrue(result);
            Assert.IsFalse(_mockContext.Heroes.ContainsKey(heroId));
        }

        [TestMethod]
        public void RemoveById_ShouldReturnFalse_WhenHeroDoesNotExist()
        {
            Guid heroId = Guid.NewGuid();

            bool result = _repository.RemoveHeroById(heroId);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Update_ShouldUpdateHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero(heroId, "Hero1", 1000, inventory);
            _mockContext.Heroes[heroId] = hero;

            DummyHero updatedHero = new DummyHero(heroId, "UpdatedHero", 1500, inventory);

            bool result = _repository.UpdateHero(heroId, updatedHero);

            Assert.IsTrue(result);
            Assert.AreEqual(updatedHero.Name, _mockContext.Heroes[heroId].Name);
        }

        [TestMethod]
        public void Update_ShouldReturnFalse_WhenHeroDoesNotExist()
        {
            Guid heroId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(10);
            DummyHero updatedHero = new DummyHero(heroId, "UpdatedHero", 1500, inventory);

            bool result = _repository.UpdateHero(heroId, updatedHero);

            Assert.IsFalse(result);
        }
    }
}