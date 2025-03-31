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

        // Child ViewModels
        public HeroSelectionViewModel HeroSelectionVM { get; private set; }
        public InventoryViewModel InventoryVM { get; private set; }
        public ShopViewModel ShopVM { get; private set; }
        public OrderViewModel OrderVM { get; private set; }

        public MainViewModel(ModelAbstractAPI modelAPI)
        {
            _modelAPI = modelAPI ?? throw new ArgumentNullException(nameof(modelAPI));

            // Initialize child ViewModels
            OrderVM = new OrderViewModel(_modelAPI);
            HeroSelectionVM = new HeroSelectionViewModel(_modelAPI);
            InventoryVM = new InventoryViewModel(_modelAPI);
            ShopVM = new ShopViewModel(_modelAPI, OrderVM);

            // Set up coordination between ViewModels
            SetupViewModelCoordination();
        }

        private void SetupViewModelCoordination()
        {
            // When hero selection changes, update other ViewModels
            // This could be done via event handling or by subscribing to PropertyChanged events

            // For example, we could implement a method in HeroSelectionViewModel:
            // public event Action<IHeroDataTransferObject> HeroChanged;

            // And then subscribe to it here:
            // HeroSelectionVM.HeroChanged += hero => {
            //     InventoryVM.HeroId = hero.Id;
            //     ShopVM.CurrentHero = hero;
            //     OrderVM.Buyer = hero;
            // };

            // Alternatively, we could just check for changes in a timer or command execution
        }

        // Method to refresh all ViewModels
        public void RefreshAll()
        {
            HeroSelectionVM.Refresh();

            if (HeroSelectionVM.SelectedHero != null)
            {
                InventoryVM.HeroId = HeroSelectionVM.SelectedHero.Id;
                InventoryVM.Refresh();

                ShopVM.CurrentHero = HeroSelectionVM.SelectedHero;
                ShopVM.Refresh();
            }
        }
    }
}
