using Presentation.Model.API;

namespace Presentation.ViewModel
{
    public interface IHeroMaintenanceService : IDisposable
    {
        void Start();
        void Stop();
    }

    public class HeroMaintenanceService : IHeroMaintenanceService
    {
        private readonly IHeroModelService _heroService;
        private readonly Func<IHeroModel?> _getSelectedHeroFunc; // get current hero from MainViewModel
        private readonly Func<Task> _refreshHeroDataAction;
        private readonly SynchronizationContext? _syncContext;
        private Timer? _maintenanceTimer;
        private readonly TimeSpan _interval;
        private bool _isProcessing = false;

        public HeroMaintenanceService(
            IHeroModelService heroService,
            Func<IHeroModel?> getSelectedHeroFunc,
            Func<Task> refreshHeroDataAction,
            SynchronizationContext? uiSyncContext,
            TimeSpan? interval = null)
        {
            _heroService = heroService ?? throw new ArgumentNullException(nameof(heroService));
            _getSelectedHeroFunc = getSelectedHeroFunc ?? throw new ArgumentNullException(nameof(getSelectedHeroFunc));
            _refreshHeroDataAction = refreshHeroDataAction ?? throw new ArgumentNullException(nameof(refreshHeroDataAction));
            _syncContext = uiSyncContext; // Can be null!!!
            _interval = interval ?? TimeSpan.FromSeconds(60); // Item meintenance interval
        }

        public void Start()
        {
            // Start the timer for mentenance
            if (_maintenanceTimer == null)
            {
                Console.WriteLine($"Starting maintenance timer with interval {_interval.TotalSeconds}s.");
                _maintenanceTimer = new Timer(
                    MaintenanceTick,
                    null, // No state object necessary
                    _interval, // delay
                    _interval  // interval
                );
            }
        }

        public void Stop()
        {
            _maintenanceTimer?.Dispose();
            _maintenanceTimer = null;
            Console.WriteLine("Maintenance timer stopped.");
        }

        private async void MaintenanceTick(object? state)
        {
            if (_isProcessing) return;

            _isProcessing = true;
            IHeroModel? currentHero = null;

            try
            {
                if (_syncContext != null)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        _syncContext.Send(_ =>
                        {
                            currentHero = _getSelectedHeroFunc(); // later on the client "selected hero" will be specific
                        }, null);
                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
                }
                else
                {
                    currentHero = _getSelectedHeroFunc();
                }


                if (currentHero == null)
                {
                    Console.WriteLine("[MaintenanceTask] No hero selected.");
                    return;
                }

                Console.WriteLine($"[MaintenanceTask] Maintenance for Hero: {currentHero.Name} ({currentHero.Id})");

                await Task.Run(() => _heroService.TriggerPeriodicItemMaintenanceDeduction());
                Console.WriteLine($"[MaintenanceTask] Meintenance task sent for processing | Hero: {currentHero.Name}");


                // refresh data on UI thread
                if (_syncContext != null)
                {
                    _syncContext.Post(async _ =>
                    {
                        try
                        {
                            // if the same hero is still selected
                            var heroAfterCheck = _getSelectedHeroFunc();
                            if (heroAfterCheck != null && heroAfterCheck.Id == currentHero.Id)
                            {
                                await _refreshHeroDataAction(); // Ask ViewModel to refresh itself
                                Console.WriteLine($"[MaintenanceTask] UI refresh for Hero: {currentHero.Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[MaintenanceTask] Error during UI refresh: {ex.Message}");
                        }

                    }, null);
                }
                else // Fallback
                {
                    await _refreshHeroDataAction();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MaintenanceTask] Error during maintenance process: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}