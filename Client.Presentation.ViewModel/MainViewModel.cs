﻿using Client.Logic.API;
using Client.Presentation.Model.API;
using Client.Presentation.ViewModel.MVVMLight;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Client.Presentation.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        #region Fields

        // Services
        private readonly IHeroModelService _heroService;
        private readonly IItemModelService _itemService;
        private readonly IOrderModelService _orderService;
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

        private bool _isConnected = false;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (SetField(ref _isConnected, value))
                {
                    ConnectToServerCommand.RaiseCanExecuteChanged();
                    DisconnectFromServerCommand.RaiseCanExecuteChanged();
                }
            }
        }


        // Commands
        public RelayCommand BuyItemCommand { get; }
        public RelayCommand ConnectToServerCommand { get; }
        public RelayCommand DisconnectFromServerCommand { get; }

        #endregion

        #region Constructors

        public MainViewModel()
            // create default services from the factory and inject them to constructor below
            : this(ModelFactory.CreateHeroModelService(),
                   ModelFactory.CreateItemModelService(),
                   ModelFactory.CreateOrderModelService())
        {
        }

        public IConnectionService _connectionService = null!;

        public MainViewModel(
            IHeroModelService heroService,
            IItemModelService itemService,
            IOrderModelService orderService)
        {
            // Initialize Commands
            BuyItemCommand = new RelayCommand(ExecuteBuyItem, CanExecuteBuyItem);
            ConnectToServerCommand = new RelayCommand(ExecuteConnectToServer, CanConnectToServer);
            DisconnectFromServerCommand = new RelayCommand(ExecuteDisconnectFromServer, CanDisconnectFromServer);

            _heroService = heroService ?? throw new ArgumentNullException(nameof(heroService));
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));

            _connectionService = LogicFactory.CreateConnectionService(null, async () =>
            {
                _ = LoadInitialDataAsync();
            });

            Task.Run(async () =>
            {
                IsConnected = true;
                await _connectionService.Connect(new Uri("ws://localhost:9081/ws"));

                // run after 2s to give time to connect
                await Task.Delay(2000).ContinueWith(async _ =>
                {
                    await _connectionService.FetchHeroes();
                    await _connectionService.FetchItems();
                    await _connectionService.FetchInventories();
                });
            });

            _syncContext = SynchronizationContext.Current;
            if (_syncContext == null)
            {
                Debug.WriteLine("Warning: MainViewModel created outside of WPF project");
            }

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

                if (heroes.Any())
                {
                    Action selectFirstHero = () => SelectedHero = heroes[0];
                    if (_syncContext != null)
                    {
                        _syncContext.Post(_ => selectFirstHero(), null);
                    }
                    else
                    {
                        selectFirstHero();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("loading heroes", ex);
            }
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

        private void ExecuteConnectToServer(object? parameter)
        {
            IsConnected = true;

            Task.Run(async () =>
            {
                await _connectionService.Connect(new Uri("ws://localhost:9081/ws"));
            });
        }

        private bool CanConnectToServer(object? parameter)
        {
            return !IsConnected;
        }

        private async void ExecuteDisconnectFromServer(object? parameter)
        {
            await _connectionService.Disconnect();

            IsConnected = false;
        }

        private bool CanDisconnectFromServer(object? parameter)
        {
            return IsConnected;
        }

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

            await _connectionService.CreateOrder(Guid.NewGuid(), buyer.Id, [itemToBuy.Id]);
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
            if (IsConnected)
                _connectionService.Disconnect().Wait();

            Debug.WriteLine("Disposing MainViewModel...");
            GC.SuppressFinalize(this);
            Debug.WriteLine("MainViewModel disposed.");
        }

        #endregion
    }
}