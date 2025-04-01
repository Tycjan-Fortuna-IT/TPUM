// --- Presentation/ViewModel/MainViewModel.cs ---
using Presentation.Model.API;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input; // For ICommand

namespace Presentation.ViewModel
{
    // MainViewModel now implements IDisposable to clean up the maintenance service
    public class MainViewModel : ViewModelBase, IDisposable
    {
        // --- Services ---
        private readonly IHeroModelService _heroService;
        private readonly IItemModelService _itemService;
        private readonly IOrderModelService _orderService;
        private readonly IHeroMaintenanceService _maintenanceService; // Use the new service
        private readonly SynchronizationContext? _syncContext; // Captured UI context

        // --- Data Collections ---
        public ObservableCollection<IHeroModel> Heroes { get; } = new ObservableCollection<IHeroModel>();
        public ObservableCollection<IItemModel> SelectedHeroInventory { get; } = new ObservableCollection<IItemModel>();
        public ObservableCollection<IItemModel> ShopItems { get; } = new ObservableCollection<IItemModel>();
        public ObservableCollection<IOrderModel> Orders { get; } = new ObservableCollection<IOrderModel>();

        // --- Selected Items ---
        private IHeroModel? _selectedHero;
        public IHeroModel? SelectedHero
        {
            get => _selectedHero;
            set
            {
                // Use SetField for INotifyPropertyChanged
                if (SetField(ref _selectedHero, value))
                {
                    // Update dependent UI properties when hero changes
                    UpdateSelectedHeroUIData();
                    // Notify commands that depend on SelectedHero
                    BuyItemCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private IItemModel? _selectedShopItem;
        public IItemModel? SelectedShopItem
        {
            get => _selectedShopItem;
            set
            {
                if (SetField(ref _selectedShopItem, value))
                {
                    // Notify commands that depend on SelectedShopItem
                    BuyItemCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // --- Display Properties ---
        private float _selectedHeroGold;
        public float SelectedHeroGold
        {
            get => _selectedHeroGold;
            // Private set ensures it's only updated internally via RefreshSelectedHeroDataAsync
            private set => SetField(ref _selectedHeroGold, value);
        }

        // --- Commands ---
        public RelayCommand BuyItemCommand { get; }

        // --- Constructor ---
        public MainViewModel() // Parameterless for View Locator / Design Time
        {
            // **Important:** This assumes the ViewModel is created on the UI thread!
            _syncContext = SynchronizationContext.Current;
            if (_syncContext == null)
            {
                Console.WriteLine("Warning: MainViewModel created without a SynchronizationContext. UI updates from background threads might fail.");
                // In some scenarios (like unit tests), you might provide a dummy context
                // or design the code to handle null context gracefully.
            }

            // Use the static factory from Model.API for simplicity here:
            var modelFactory = ModelFactory.CreateFactory();
            _heroService = modelFactory.CreateHeroModelService();
            _itemService = modelFactory.CreateItemModelService();
            _orderService = modelFactory.CreateOrderModelService();

            // Create the maintenance service, passing delegates/funcs to interact with this VM
            _maintenanceService = new HeroMaintenanceService(
                _heroService,
                GetSelectedHeroForMaintenance, // Method to get current hero
                RefreshSelectedHeroDataAsync, // Method to trigger refresh
                _syncContext); // Pass the captured UI context

            BuyItemCommand = new RelayCommand(ExecuteBuyItem, CanExecuteBuyItem);

            // Load initial data asynchronously
            _ = LoadInitialDataAsync();
            // Start the background maintenance task
            _maintenanceService.Start();
        }

        // Constructor for Dependency Injection (Preferred) - Omitted for brevity,
        // but would inject services and create HeroMaintenanceService similarly.

        // --- Data Loading and Refreshing ---

        private async Task LoadInitialDataAsync()
        {
            await LoadHeroesAsync();
            await LoadShopItemsAsync();
            await RefreshOrdersAsync();
        }

        private async Task LoadHeroesAsync()
        {
            try
            {
                var heroes = await Task.Run(() => _heroService.GetAllHeroes().ToList());
                UpdateObservableCollection(Heroes, heroes); // Use helper to update on UI thread
            }
            catch (Exception ex) { LogError("loading heroes", ex); }
        }

        private async Task LoadShopItemsAsync()
        {
            try
            {
                var items = await Task.Run(() => _itemService.GetAllItems().ToList());
                UpdateObservableCollection(ShopItems, items);
            }
            catch (Exception ex) { LogError("loading shop items", ex); }
        }

        public async Task RefreshOrdersAsync() // Made public if external refresh needed
        {
            try
            {
                var orders = await Task.Run(() => _orderService.GetAllOrders().ToList());
                UpdateObservableCollection(Orders, orders);
            }
            catch (Exception ex) { LogError("refreshing orders", ex); }
        }

        // Central method to update ObservableCollections safely on the UI thread
        private void UpdateObservableCollection<T>(ObservableCollection<T> collection, List<T> newData)
        {
            Action updateAction = () => {
                collection.Clear();
                foreach (var item in newData)
                {
                    collection.Add(item);
                }
                // --- REMOVED THE PROBLEMATIC 'if' BLOCK ---
                // If you *did* want to auto-select the first hero, you'd handle it
                // specifically after calling UpdateObservableCollection for Heroes:
                // e.g., after LoadHeroesAsync completes:
                // UpdateObservableCollection(Heroes, heroes);
                // if (Heroes.Count > 0 && SelectedHero == null) SelectedHero = Heroes[0];
            };

            // Post the update to the UI thread if context exists, otherwise run directly
            if (_syncContext != null)
            {
                _syncContext.Post(_ => updateAction(), null);
            }
            else
            {
                // Run directly if no context (e.g., tests or non-UI thread construction)
                updateAction();
            }
        }


        // Refreshes the currently selected hero's Gold and Inventory
        // This method MUST be called on the UI thread (or marshalled to it)
        // because it modifies UI-bound properties.
        public async Task RefreshSelectedHeroDataAsync()
        {
            IHeroModel? currentHero = _selectedHero; // Capture locally
            if (currentHero == null) return;

            Console.WriteLine($"RefreshSelectedHeroDataAsync: Refreshing data for {currentHero.Name}");
            try
            {
                // Fetch potentially updated data from the service (can run in background)
                var updatedHeroData = await Task.Run(() => _heroService.GetHero(currentHero.Id));

                // Update UI properties on the UI thread
                Action updateAction = () => {
                    // Double-check the hero hasn't changed *again* since the Task.Run started
                    if (_selectedHero == null || _selectedHero.Id != currentHero.Id)
                    {
                        Console.WriteLine($"RefreshSelectedHeroDataAsync: Hero changed before UI update for {currentHero.Name}. Aborting UI update.");
                        return; // Hero changed, don't update stale UI
                    }

                    if (updatedHeroData != null)
                    {
                        SelectedHeroGold = updatedHeroData.Gold; // Update bound property
                        SelectedHeroInventory.Clear();
                        foreach (var item in updatedHeroData.Inventory.Items)
                        {
                            SelectedHeroInventory.Add(item);
                        }
                        Console.WriteLine($"RefreshSelectedHeroDataAsync: UI updated for {currentHero.Name}. New Gold: {SelectedHeroGold}");
                    }
                    else
                    {
                        // Hero might have been deleted in the meantime
                        Console.WriteLine($"RefreshSelectedHeroDataAsync: Hero {currentHero.Name} not found after refresh. Clearing selection.");
                        SelectedHero = null; // Clear selection, which will clear UI via setter
                    }
                };

                // Post the update action back to the UI thread
                _syncContext?.Post(_ => updateAction(), null);

            }
            catch (Exception ex)
            {
                LogError($"refreshing data for hero {currentHero.Name}", ex);
            }
        }

        // Updates UI display based on the _selectedHero field. Called by SelectedHero setter.
        // Runs directly on UI thread as it's called from property setter.
        private void UpdateSelectedHeroUIData()
        {
            if (_selectedHero != null)
            {
                SelectedHeroGold = _selectedHero.Gold;
                SelectedHeroInventory.Clear();
                foreach (var item in _selectedHero.Inventory.Items)
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

        // --- Command Implementations ---

        private bool CanExecuteBuyItem(object? parameter)
        {
            return SelectedHero != null && SelectedShopItem != null;
        }

        private async void ExecuteBuyItem(object? parameter)
        {
            // Capture state needed for the operation
            IHeroModel? buyer = SelectedHero;
            IItemModel? itemToBuy = SelectedShopItem;

            if (buyer == null || itemToBuy == null) return; // Should be handled by CanExecute

            Console.WriteLine($"ExecuteBuyItem: Attempting purchase for Hero {buyer.Name}, Item {itemToBuy.Name}");
            try
            {
                Guid orderId = Guid.NewGuid();
                List<Guid> itemIds = new List<Guid> { itemToBuy.Id };

                // Perform the service call in the background
                await Task.Run(() => _orderService.AddOrder(orderId, buyer.Id, itemIds));
                Console.WriteLine($"ExecuteBuyItem: Order {orderId} added via service.");

                // Simulate immediate processing (replace with actual server logic later)
                await Task.Run(() => _orderService.TriggerPeriodicOrderProcessing());
                Console.WriteLine($"ExecuteBuyItem: Order processing triggered.");


                // Refresh relevant UI data after the operation completes
                // Post the refresh calls back to the UI thread context
                _syncContext?.Post(async _ => {
                    await RefreshOrdersAsync();
                    // Check if the buyer is still the selected hero before refreshing them
                    if (SelectedHero != null && SelectedHero.Id == buyer.Id)
                    {
                        await RefreshSelectedHeroDataAsync();
                    }
                    Console.WriteLine($"ExecuteBuyItem: UI refreshed after purchase.");
                }, null);

            }
            catch (InvalidOperationException ioex)
            {
                LogError($"creating order for Hero {buyer.Name}, Item {itemToBuy.Name}", ioex);
                // TODO: Show user feedback (e.g., "Item not found", "Not enough gold?")
            }
            catch (Exception ex)
            {
                LogError($"buying item for Hero {buyer.Name}", ex);
                // TODO: Show generic error feedback
            }
        }

        // --- Methods for Maintenance Service ---

        // Provides the currently selected hero to the maintenance service.
        // This method will be called *by* the maintenance service *on the UI thread* via syncContext.Send.
        private IHeroModel? GetSelectedHeroForMaintenance()
        {
            // Directly return the current value - this runs on UI thread context
            // Console.WriteLine($"GetSelectedHeroForMaintenance: Providing hero: {(_selectedHero?.Name ?? "None")}");
            return _selectedHero;
        }

        // --- Utility ---
        private void LogError(string action, Exception ex)
        {
            // Implement proper logging later
            Console.WriteLine($"Error during {action}: {ex.Message}");
            // Console.WriteLine(ex.StackTrace); // Optional: include stack trace for debugging
        }


        // --- Cleanup ---
        public void Dispose()
        {
            Console.WriteLine("Disposing MainViewModel...");
            // Stop and dispose the maintenance service and its timer
            _maintenanceService?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}