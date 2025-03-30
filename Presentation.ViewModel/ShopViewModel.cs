using Presentation.Model.API;
using Presentation.ViewModel.MVVMLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    public class ShopViewModel : ViewModelBase
    {
        private readonly ModelAbstractAPI _modelAPI;
        private readonly OrderViewModel _orderViewModel;
        private ObservableCollection<ShopItemViewModel> _shopItems;
        private ShopItemViewModel? _selectedShopItem;
        private int _purchaseQuantity;

        public ShopViewModel(ModelAbstractAPI modelAPI, OrderViewModel orderViewModel)
        {
            _modelAPI = modelAPI ?? throw new ArgumentNullException(nameof(modelAPI));
            _orderViewModel = orderViewModel ?? throw new ArgumentNullException(nameof(orderViewModel));
            _shopItems = new ObservableCollection<ShopItemViewModel>();
            _purchaseQuantity = 1;

            // Commands
            PurchaseCommand = new RelayCommand(ExecutePurchase, CanExecutePurchase);

            LoadShopItems();
        }

        public ObservableCollection<ShopItemViewModel> ShopItems
        {
            get => _shopItems;
            set => SetProperty(ref _shopItems, value);
        }

        public ShopItemViewModel? SelectedShopItem
        {
            get => _selectedShopItem;
            set
            {
                if (SetProperty(ref _selectedShopItem, value))
                {
                    // Reset quantity when new item is selected
                    PurchaseQuantity = 1;
                    // Refresh command state
                    ((RelayCommand)PurchaseCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public int PurchaseQuantity
        {
            get => _purchaseQuantity;
            set
            {
                if (value < 1) value = 1;
                if (SetProperty(ref _purchaseQuantity, value))
                {
                    // Refresh command state
                    ((RelayCommand)PurchaseCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand PurchaseCommand { get; }

        private void LoadShopItems()
        {
            var items = _modelAPI.GetAllItems();
            var shopItems = new ObservableCollection<ShopItemViewModel>();

            foreach (var item in items)
            {
                shopItems.Add(new ShopItemViewModel
                {
                    ItemId = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Value,
                    Stock = item.Stock
                });
            }

            ShopItems = shopItems;
        }

        private bool CanExecutePurchase(object? parameter)
        {
            if (SelectedShopItem == null || _orderViewModel.CurrentHeroId == Guid.Empty)
                return false;

            return SelectedShopItem.Stock >= PurchaseQuantity;
        }

        private void ExecutePurchase(object? parameter)
        {
            if (SelectedShopItem == null || _orderViewModel.CurrentHeroId == Guid.Empty)
                return;

            // Create a new order
            _orderViewModel.CreateOrder(SelectedShopItem.ItemId, PurchaseQuantity);

            // Refresh the shop items
            LoadShopItems();
        }

        public void RefreshShopItems()
        {
            LoadShopItems();
        }
    }

    public class ShopItemViewModel : ViewModelBase
    {
        private Guid _itemId;
        private string _name;
        private string _description;
        private decimal _price;
        private int _stock;

        public Guid ItemId
        {
            get => _itemId;
            set => SetProperty(ref _itemId, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public int Stock
        {
            get => _stock;
            set => SetProperty(ref _stock, value);
        }
    }
}
