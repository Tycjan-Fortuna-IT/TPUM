using Presentation.Model.API;
using Presentation.Model.Tests;

namespace Presentation.ViewModel.Tests
{
    [TestClass]
    public sealed class MainViewModelTests
    {
        private IInventoryModelService _inventoryModelService = null!;
        private IHeroModelService _heroModelService = null!;
        private IItemModelService _itemModelService = null!;
        private IOrderModelService _orderModelService = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _inventoryModelService = new DummyInventoryModelService();
            _heroModelService = new DummyHeroModelService(_inventoryModelService);
            _itemModelService = new DummyItemModelService();
            _orderModelService = new DummyOrderModelService(_heroModelService, _itemModelService);
        }

        [TestMethod]
        public void Create_TestMethod()
        {
            MainViewModel vm = new MainViewModel(_heroModelService, _itemModelService, _orderModelService);

            Assert.IsNotNull(vm);
            Assert.IsFalse(vm.Heroes.Any());
            Assert.IsFalse(vm.ShopItems.Any());
            Assert.IsFalse(vm.SelectedHeroInventory.Any());
            Assert.IsFalse(vm.Orders.Any());

            Assert.IsNull(vm.SelectedHero);
            Assert.IsNull(vm.SelectedShopItem);

            Assert.IsFalse(vm.BuyItemCommand.CanExecute(null));
        }

        [TestMethod]
        public void Action_TestMethod()
        {
            MainViewModel vm = new MainViewModel(_heroModelService, _itemModelService, _orderModelService);

            Assert.IsFalse(vm.BuyItemCommand.CanExecute(null));

            Guid invGuid = Guid.NewGuid();

            _inventoryModelService.Add(invGuid, 25);

            Guid heroGuid = Guid.NewGuid();

            _heroModelService.AddHero(heroGuid, "Test", 200, invGuid);

            IHeroModel? hero = _heroModelService.GetHero(heroGuid);
            Assert.IsNotNull(hero);

            Guid itemGuid = Guid.NewGuid();
            _itemModelService.AddItem(itemGuid, "Test", 15, 10);

            IItemModel? item = _itemModelService.GetItem(itemGuid);

            Assert.IsFalse(vm.BuyItemCommand.CanExecute(null));

            vm.SelectedHero = hero;
            vm.SelectedShopItem = item;

            Assert.IsTrue(vm.BuyItemCommand.CanExecute(null));
        }
    }
}
