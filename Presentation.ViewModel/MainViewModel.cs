using Presentation.Model.API;
using Presentation.ViewModel.MVVMLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ModelAbstractAPI _modelAPI;
        public HeroSelectionViewModel HeroSelectionVM { get; }
        public InventoryViewModel InventoryVM { get; }
        public ShopViewModel ShopVM { get; }
        public OrderViewModel OrderVM { get; }

        public MainViewModel(ModelAbstractAPI modelAPI)
        {
            _modelAPI = modelAPI ?? throw new ArgumentNullException(nameof(modelAPI));

            // Initialize ViewModels in the correct order to handle dependencies
            OrderVM = new OrderViewModel(modelAPI);
            InventoryVM = new InventoryViewModel(modelAPI);
            HeroSelectionVM = new HeroSelectionViewModel(modelAPI, InventoryVM, OrderVM);
            ShopVM = new ShopViewModel(modelAPI, OrderVM);

            // Commands for main window operations
            RefreshDataCommand = new RelayCommand(ExecuteRefreshData);
            ProcessOrdersCommand = new RelayCommand(ExecuteProcessOrders);
            PerformMaintenanceCommand = new RelayCommand(ExecutePerformMaintenance);
            RestockItemsCommand = new RelayCommand(ExecuteRestockItems);
        }

        public ICommand RefreshDataCommand { get; }
        public ICommand ProcessOrdersCommand { get; }
        public ICommand PerformMaintenanceCommand { get; }
        public ICommand RestockItemsCommand { get; }

        private void ExecuteRefreshData(object? parameter)
        {
            HeroSelectionVM.RefreshHeroes();
            InventoryVM.RefreshInventory();
            ShopVM.RefreshShopItems();
            OrderVM.LoadHeroOrders(OrderVM.CurrentHeroId);
        }

        private void ExecuteProcessOrders(object? parameter)
        {
            OrderVM.ProcessOrders();
            ExecuteRefreshData(null);
        }

        private void ExecutePerformMaintenance(object? parameter)
        {
            _modelAPI.PerformHeroMaintenance();
            ExecuteRefreshData(null);
        }

        private void ExecuteRestockItems(object? parameter)
        {
            _modelAPI.RestockItems();
            ShopVM.RefreshShopItems();
        }
    }
}
