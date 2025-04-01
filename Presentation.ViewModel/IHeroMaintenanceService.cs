// --- Presentation/ViewModel/HeroMaintenanceService.cs ---
using System;
using System.Threading;
using System.Threading.Tasks;
using Presentation.Model.API; // For IHeroModelService, IHeroModel

namespace Presentation.ViewModel
{
    // Interface for the service (optional but good practice)
    public interface IHeroMaintenanceService : IDisposable
    {
        void Start();
        void Stop();
    }

    // Implementation
    public class HeroMaintenanceService : IHeroMaintenanceService
    {
        private readonly IHeroModelService _heroService;
        private readonly Func<IHeroModel?> _getSelectedHeroFunc; // Function to get current hero from MainViewModel
        private readonly Func<Task> _refreshHeroDataAction; // Action to tell MainViewModel to refresh
        private readonly SynchronizationContext? _syncContext; // UI context
        private Timer? _maintenanceTimer;
        private readonly TimeSpan _interval;
        private bool _isProcessing = false; // Prevent re-entrancy

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
            _syncContext = uiSyncContext; // Can be null if not run on UI thread, handle gracefully
            _interval = interval ?? TimeSpan.FromSeconds(60); // Default to 60 seconds
        }

        public void Start()
        {
            // Start the timer only if it's not already running
            if (_maintenanceTimer == null)
            {
                Console.WriteLine($"Starting maintenance timer with interval {_interval.TotalSeconds}s.");
                // Use System.Threading.Timer for background execution
                _maintenanceTimer = new Timer(
                    MaintenanceTick,
                    null, // No state object needed
                    _interval, // Initial delay same as interval
                    _interval  // Interval
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
            if (_isProcessing) return; // Don't run if already processing

            _isProcessing = true;
            IHeroModel? currentHero = null;

            try
            {
                // --- Step 1: Get the current hero safely on the UI thread ---
                if (_syncContext != null)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        _syncContext.Send(_ => // Send waits for completion
                        {
                            currentHero = _getSelectedHeroFunc();
                        }, null);
                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default); // Run Send on a pool thread
                }
                else // Fallback if no sync context (e.g., testing) - assumes direct call is safe
                {
                    currentHero = _getSelectedHeroFunc();
                }


                if (currentHero == null)
                {
                    // Console.WriteLine("MaintenanceTick: No hero selected.");
                    return; // No hero selected, do nothing this tick
                }

                Console.WriteLine($"MaintenanceTick: Processing maintenance for Hero: {currentHero.Name} ({currentHero.Id})");

                // --- Step 2: Trigger the logic layer processing (can run off UI thread) ---
                await Task.Run(() => _heroService.TriggerPeriodicItemMaintenanceDeduction());
                Console.WriteLine($"MaintenanceTick: Logic layer processing triggered for Hero: {currentHero.Name}");


                // --- Step 3: Request the MainViewModel to refresh data on UI thread ---
                if (_syncContext != null)
                {
                    _syncContext.Post(async _ =>
                    {
                        try
                        {
                            // Check again if the same hero is still selected before refreshing
                            var heroAfterCheck = _getSelectedHeroFunc();
                            if (heroAfterCheck != null && heroAfterCheck.Id == currentHero.Id)
                            {
                                Console.WriteLine($"MaintenanceTick: Requesting UI refresh for Hero: {currentHero.Name}");
                                await _refreshHeroDataAction(); // Ask ViewModel to refresh itself
                                Console.WriteLine($"MaintenanceTick: UI refresh completed for Hero: {currentHero.Name}");
                            }
                            else
                            {
                                Console.WriteLine($"MaintenanceTick: Hero changed during processing, skipping refresh.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error during UI refresh callback: {ex.Message}");
                        }

                    }, null);
                }
                else // Fallback if no context
                {
                    await _refreshHeroDataAction();
                }

            }
            catch (Exception ex)
            {
                // Log the error, but don't crash the timer
                Console.WriteLine($"Error during maintenance tick: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
            finally
            {
                _isProcessing = false; // Allow next tick
            }
        }

        public void Dispose()
        {
            Stop(); // Ensure timer is stopped and disposed
            GC.SuppressFinalize(this);
        }
    }
}