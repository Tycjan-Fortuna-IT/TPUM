using Presentation.ViewModel.MVVMLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Presentation.Model.API;

namespace Presentation.ViewModel
{
    public class HeroSelectionViewModel : ViewModelBase
    {
        private readonly ModelAbstractAPI _modelAPI;
        private readonly InventoryViewModel _inventoryViewModel;
        private readonly OrderViewModel _orderViewModel;
        private IHeroModel? _selectedHero;
        private ObservableCollection<IHeroModel> _heroes;

        public HeroSelectionViewModel(ModelAbstractAPI modelAPI, InventoryViewModel inventoryViewModel, OrderViewModel orderViewModel)
        {
            _modelAPI = modelAPI ?? throw new ArgumentNullException(nameof(modelAPI));
            _inventoryViewModel = inventoryViewModel ?? throw new ArgumentNullException(nameof(inventoryViewModel));
            _orderViewModel = orderViewModel ?? throw new ArgumentNullException(nameof(orderViewModel));
            _heroes = new ObservableCollection<IHeroModel>(_modelAPI.GetAllHeroes());
        }

        public ObservableCollection<IHeroModel> Heroes
        {
            get => _heroes;
            set => SetProperty(ref _heroes, value);
        }

        public IHeroModel? SelectedHero
        {
            get => _selectedHero;
            set
            {
                if (SetProperty(ref _selectedHero, value) && _selectedHero != null)
                {
                    // Update related ViewModels
                    _inventoryViewModel.LoadHeroInventory(_selectedHero.Id);
                    _orderViewModel.CurrentHeroId = _selectedHero.Id;
                    OnPropertyChanged(nameof(GoldBalance));
                }
            }
        }

        public float GoldBalance => _selectedHero?.Gold ?? 0;

        public void RefreshHeroes()
        {
            Heroes = new ObservableCollection<IHeroModel>(_modelAPI.GetAllHeroes());
            OnPropertyChanged(nameof(GoldBalance));
        }
    }
}
