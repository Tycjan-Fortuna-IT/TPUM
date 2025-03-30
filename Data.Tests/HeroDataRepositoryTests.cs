using Data.API;

namespace Data.Tests
{
    [TestClass]
    public class HeroDataRepositoryTests
    {
        private IDataContext _context = default!;
        private IDataRepository<IHero> _repository = default!;

        [TestInitialize]
        public void SetUp()
        {
            _context = DataContextFactory.CreateDataContext();
            _repository = DataRepositoryFactory.CreateHeroRepository(_context);
        }

        [TestMethod]
        public void Add_ShouldAddHeroToRepository()
        {
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero("Hero1", 1000, inventory);

            _repository.Add(hero);

            Assert.IsTrue(_context.Heroes.ContainsKey(hero.Id));
            Assert.AreEqual(hero.Name, _context.Heroes[hero.Id].Name);
        }

        [TestMethod]
        public void Get_ShouldReturnHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero(heroId, "Hero1", 1000, inventory);

            _context.Heroes[heroId] = hero;

            IHero? result = _repository.Get(heroId);

            Assert.IsNotNull(result);
            Assert.AreEqual(hero.Name, result.Name);
        }

        [TestMethod]
        public void Get_ShouldReturnNull_WhenHeroDoesNotExist()
        {
            Guid heroId = Guid.NewGuid();

            IHero? result = _repository.Get(heroId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Remove_ShouldRemoveHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero(heroId, "Hero1", 1000, inventory);
            _context.Heroes[heroId] = hero;

            bool result = _repository.Remove(hero);

            Assert.IsTrue(result);
            Assert.IsFalse(_context.Heroes.ContainsKey(heroId));
        }

        [TestMethod]
        public void Remove_ShouldReturnFalse_WhenHeroDoesNotExist()
        {
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero(Guid.NewGuid(), "Hero1", 1000, inventory);

            bool result = _repository.Remove(hero);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RemoveById_ShouldRemoveHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero(heroId, "Hero1", 1000, inventory);
            _context.Heroes[heroId] = hero;

            bool result = _repository.RemoveById(heroId);

            Assert.IsTrue(result);
            Assert.IsFalse(_context.Heroes.ContainsKey(heroId));
        }

        [TestMethod]
        public void RemoveById_ShouldReturnFalse_WhenHeroDoesNotExist()
        {
            Guid heroId = Guid.NewGuid();

            bool result = _repository.RemoveById(heroId);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Update_ShouldUpdateHero_WhenHeroExists()
        {
            Guid heroId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(10);
            DummyHero hero = new DummyHero(heroId, "Hero1", 1000, inventory);
            _context.Heroes[heroId] = hero;

            DummyHero updatedHero = new DummyHero(heroId, "UpdatedHero", 1500, inventory);

            bool result = _repository.Update(heroId, updatedHero);

            Assert.IsTrue(result);
            Assert.AreEqual(updatedHero.Name, _context.Heroes[heroId].Name);
        }

        [TestMethod]
        public void Update_ShouldReturnFalse_WhenHeroDoesNotExist()
        {
            Guid heroId = Guid.NewGuid();
            DummyInventory inventory = new DummyInventory(10);
            DummyHero updatedHero = new DummyHero(heroId, "UpdatedHero", 1500, inventory);

            bool result = _repository.Update(heroId, updatedHero);

            Assert.IsFalse(result);
        }
    }
}
