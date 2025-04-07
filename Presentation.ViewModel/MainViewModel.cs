using Client.Presentation.ViewModel.MVVMLight;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Client.Presentation.Model.API;

namespace Client.Presentation.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        #region Fields

        // Services
        private readonly IHeroModelService _heroService;
        private readonly IItemModelService _itemService;
        private readonly IOrderModelService _orderService;
        private readonly IHeroMaintenanceService _maintenanceService;
        private readonly SynchronizationContext? _syncContext;

        private IHeroModel? _selectedHero;
        private IItemModel? _selectedShopItem;
        private float _selectedHeroGold;

        #endregion

        #region Properties

        public ObservableCollection<IHeroModel> Heroes { get; } = new ObservableCollection<IHeroModel>();
        public ObservableCollection<IItemModel> SelectedHeroInventory { get; } = new ObservableCollection<IItemModel>();
        public ObservableCollection<IItemModel> ShopItems { get; } = new ObservableCollection<IItemModel>();
        public ObservableCollection<IOrderModel> Orders { get; } = new ObservableCollection<IOrderModel>();

        // Selected Hero
        public IHeroModel? SelectedHero
        {
            get => _selectedHero;
            set
            {
                if (SetField(ref _selectedHero, value))
                {
                    UpdateSelectedHeroUIData();
                    BuyItemCommand.RaiseCanExecuteChanged(); // Update connected commands
                }
            }
        }

        // Selected Item
        public IItemModel? SelectedShopItem
        {
            get => _selectedShopItem;
            set
            {
                if (SetField(ref _selectedShopItem, value))
                {
                    BuyItemCommand.RaiseCanExecuteChanged(); // Update connected commands
                }
            }
        }

        public float SelectedHeroGold
        {
            get => _selectedHeroGold;
            private set => SetField(ref _selectedHeroGold, value);
        }

        // Commands
        public RelayCommand BuyItemCommand { get; }

        #endregion

        #region Constructors

        public MainViewModel()
            // create default services from the factory and inject them to constructor below
            : this(ModelFactory.CreateHeroModelService(),
                   ModelFactory.CreateItemModelService(),
                   ModelFactory.CreateOrderModelService())
        {
        }

        public MainViewModel(
            IHeroModelService heroService,
            IItemModelService itemService,
            IOrderModelService orderService)
        {

            _syncContext = SynchronizationContext.Current;
            if (_syncContext == null)
            {
                Debug.WriteLine("Warning: MainViewModel created outside of WPF project");
            }

            _heroService = heroService ?? throw new ArgumentNullException(nameof(heroService));
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));

            // Create the maintenance service
            _maintenanceService = new HeroMaintenanceService(
                _heroService,
                GetSelectedHeroForMaintenance,
                RefreshSelectedHeroDataAsync,
                _syncContext);

            // Initialize Commands
            BuyItemCommand = new RelayCommand(ExecuteBuyItem, CanExecuteBuyItem);

            _ = LoadInitialDataAsync();

            // Start the background maintenance task
            _maintenanceService.Start();

            Debug.WriteLine("MainViewModel creation complete.");
        }

        #endregion

        #region Data Loading

        private async Task LoadInitialDataAsync()
        {
            await Task.WhenAll(
                LoadHeroesAsync(),
                LoadShopItemsAsync(),
                RefreshOrdersAsync()
            );
            Debug.WriteLine("Initial data loading complete.");
        }

        private async Task LoadHeroesAsync()
        {
            try
            {
                List<IHeroModel> heroes = await Task.Run(() => _heroService.GetAllHeroes().ToList());
                UpdateObservableCollection(Heroes, heroes);
            }
            catch (Exception ex) { LogError("loading heroes", ex); }
        }

        private async Task LoadShopItemsAsync()
        {
            try
            {
                List<IItemModel> items = await Task.Run(() => _itemService.GetAllItems().ToList());
                UpdateObservableCollection(ShopItems, items);
            }
            catch (Exception ex) { LogError("loading shop items", ex); }
        }

        public async Task RefreshOrdersAsync()
        {
            try
            {
                List<IOrderModel> orders = await Task.Run(() => _orderService.GetAllOrders().ToList());
                UpdateObservableCollection(Orders, orders);
            }
            catch (Exception ex) { LogError("refreshing orders", ex); }
        }

        public void UpdateObservableCollection<T>(ObservableCollection<T> collection, List<T> newData)
        {
            Action updateAction = () =>
            {
                collection.Clear();
                foreach (T item in newData)
                {
                    collection.Add(item);
                }
            };

            if (_syncContext != null)
            {
                _syncContext.Post(_ => updateAction(), null);
            }
            else
            {
                updateAction();
            }
        }


        // Refreshes Gold and Inventory
        private async Task RefreshSelectedHeroDataAsync()
        {
            IHeroModel? currentHero = _selectedHero;
            if (currentHero == null) return;

            try
            {
                IHeroModel? updatedHeroData = await Task.Run(() => _heroService.GetHero(currentHero.Id));

                // Prepare the UI update
                Action updateAction = () =>
                {
                    if (_selectedHero == null || _selectedHero.Id != currentHero.Id)
                    {
                        return;
                    }

                    if (updatedHeroData != null)
                    {
                        SelectedHeroGold = updatedHeroData.Gold;
                        SelectedHeroInventory.Clear();
                        foreach (IItemModel item in updatedHeroData.Inventory.Items)
                        {
                            SelectedHeroInventory.Add(item);
                        }
                        Debug.WriteLine($"UI updated for {currentHero.Name}. New Gold: {SelectedHeroGold}");
                    }
                    else
                    {
                        SelectedHero = null;
                    }
                };

                // Post the update to the UI
                if (_syncContext != null)
                {
                    _syncContext.Post(_ => updateAction(), null);
                }
                else
                {
                    updateAction(); // Execute directly if no context
                }
            }
            catch (Exception ex)
            {
                LogError($"While refreshing data for hero {currentHero.Name}", ex);
            }
        }


        private void UpdateSelectedHeroUIData()
        {
            if (_selectedHero != null)
            {
                SelectedHeroGold = _selectedHero.Gold;
                SelectedHeroInventory.Clear();
                foreach (IItemModel item in _selectedHero.Inventory.Items)
                {
                    SelectedHeroInventory.Add(item);
                }
            }
            else
            {
                SelectedHeroGold = 0;
                SelectedHeroInventory.Clear();
            }
        }

        #endregion

        #region Command Implementations

        private bool CanExecuteBuyItem(object? parameter)
        {
            // Can only buy if a hero and item are selected
            return SelectedHero != null && SelectedShopItem != null;
        }

        private async void ExecuteBuyItem(object? parameter)
        {
            IHeroModel? buyer = SelectedHero;
            IItemModel? itemToBuy = SelectedShopItem;
            if (buyer == null || itemToBuy == null)
            {
                Debug.WriteLine($"Cannot buy item, buyer or item is null.");
                return;
            }

            Debug.WriteLine($"Attempting purchase for {buyer.Name}, Item {itemToBuy.Name}");
            try
            {
                Guid orderId = Guid.NewGuid();
                List<Guid> itemIds = new List<Guid> { itemToBuy.Id };

                await Task.Run(() => _orderService.AddOrder(orderId, buyer.Id, itemIds));
                await Task.Run(() => _orderService.TriggerPeriodicOrderProcessing());
                Debug.WriteLine($"Order processing on.");

                Func<Task> refreshAction = async () =>
                {
                    await RefreshOrdersAsync();
                    if (SelectedHero != null && SelectedHero.Id == buyer.Id)
                    {
                        await RefreshSelectedHeroDataAsync();
                    }
                };

                if (_syncContext != null)
                {
                    _syncContext.Post(async _ => await refreshAction(), null);
                }
                else
                {
                    // Execute directly if no context
                    await refreshAction();
                }


                await LoadShopItemsAsync();
            }
            catch (Exception ex)
            {
                LogError($"while buying item for Hero {buyer.Name}", ex);
                // maybe a UI callback should be made to say to the user that purchase failed
            }
        }

        #endregion


        #region Utility & Cleanup

        private IHeroModel? GetSelectedHeroForMaintenance() => _selectedHero;

        private void LogError(string action, Exception ex)
        {
            Debug.WriteLine($"Error during {action}: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}");
        }

        public void Dispose()
        {
            Debug.WriteLine("Disposing MainViewModel...");
            _maintenanceService?.Dispose();
            GC.SuppressFinalize(this);
            Debug.WriteLine("MainViewModel disposed.");
        }

        #endregion
    }
}