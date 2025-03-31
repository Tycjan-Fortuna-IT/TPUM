using Presentation.Model.API;
using Presentation.ViewModel.MVVMLight;
using System.Collections.ObjectModel;

namespace Presentation.ViewModel
{
    public class ShopViewModel : ViewModelBase
    {
        private readonly ModelAbstractAPI _modelAPI;
        private readonly OrderViewModel _orderViewModel;

        private ObservableCollection<IItemModel> _availableItems;
        private IItemModel _selectedShopItem;
        private string _shopStatus;
        private IHeroModel _currentHero;

        public ShopViewModel(ModelAbstractAPI modelAPI, OrderViewModel orderViewModel)
        {
            _modelAPI = modelAPI ?? throw new ArgumentNullException(nameof(modelAPI));
            _orderViewModel = orderViewModel ?? throw new ArgumentNullException(nameof(orderViewModel));

            // Initialize collections
            AvailableItems = new ObservableCollection<IItemModel>();

            // Load available items
            LoadAvailableItems();
        }

        // Available items collection property
        public ObservableCollection<IItemModel> AvailableItems
        {
            get => _availableItems;
            private set => SetProperty(ref _availableItems, value);
        }

        // Selected shop item property
        public IItemModel SelectedShopItem
        {
            get => _selectedShopItem;
            set => SetProperty(ref _selectedShopItem, value);
        }

        // Shop status property
        public string ShopStatus
        {
            get => _shopStatus;
            private set => SetProperty(ref _shopStatus, value);
        }

        // Current hero property
        public IHeroModel CurrentHero
        {
            get => _currentHero;
            set
            {
                if (SetProperty(ref _currentHero, value))
                {
                    // Update the order buyer
                    _orderViewModel.Buyer = _currentHero;
                    UpdateShopStatus();
                }
            }
        }

        // Method to load all available items from the model API
        public void LoadAvailableItems()
        {
            AvailableItems.Clear();

            var itemModels = _modelAPI.GetAllItems();
            if (itemModels != null)
            {
                foreach (var itemModel in itemModels)
                {
                    var itemDto = itemModel as IItemModel;
                    if (itemDto != null)
                    {
                        AvailableItems.Add(itemDto);
                    }
                }
            }

            UpdateShopStatus();
        }

        // Method to add the selected item to the order
        public void AddSelectedItemToOrder()
        {
            if (SelectedShopItem == null || CurrentHero == null)
                return;

            // Check if the hero has enough gold
            if (CurrentHero.Gold < SelectedShopItem.Price)
            {
                ShopStatus = $"Not enough gold! Item costs {SelectedShopItem.Price}, but you only have {CurrentHero.Gold}.";
                return;
            }

            // Add the item to the order
            _orderViewModel.AddItemToOrder(SelectedShopItem);
            ShopStatus = $"Added {SelectedShopItem.Name} to your order.";
        }

        // Method to update the shop status text
        private void UpdateShopStatus()
        {
            if (CurrentHero == null)
            {
                ShopStatus = "No hero selected";
                return;
            }

            ShopStatus = $"Welcome to the shop! You have {CurrentHero.Gold} gold available.";
        }

        // Method to handle hero selection from another view model
        public void HeroSelected(IHeroModel hero)
        {
            CurrentHero = hero;
        }

        // Method to refresh the shop (e.g., after restocking)
        public void RestockShop()
        {
            _modelAPI.RestockItems();
            LoadAvailableItems();
            ShopStatus = "Shop has been restocked!";
        }

        // Method to refresh the view model
        public void Refresh()
        {
            LoadAvailableItems();

            if (CurrentHero != null)
            {
                // Refresh hero data
                var hero = _modelAPI.GetHero(CurrentHero.Id);
                if (hero != null)
                {
                    CurrentHero = hero as IHeroModel;
                }
            }
        }
    }
}
