using Presentation.ViewModel.MVVMLight;
using System.Collections.ObjectModel;
using Presentation.Model.API;

namespace Presentation.ViewModel
{
    public class HeroSelectionViewModel : ViewModelBase
    {
        private readonly ModelAbstractAPI _modelAPI;

        private ObservableCollection<IHeroModel> _heroes;
        private IHeroModel _selectedHero;
        private float _availableGold;
        private ObservableCollection<IItemModel> _inventoryItems;
        private int _inventoryCapacity;

        public HeroSelectionViewModel(ModelAbstractAPI modelAPI)
        {
            _modelAPI = modelAPI ?? throw new ArgumentNullException(nameof(modelAPI));

            Heroes = new ObservableCollection<IHeroModel>();
            InventoryItems = new ObservableCollection<IItemModel>();

            LoadHeroes();
        }

        // Heroes collection property
        public ObservableCollection<IHeroModel> Heroes
        {
            get => _heroes;
            private set => SetProperty(ref _heroes, value);
        }

        // Selected hero property with logic to update related properties
        public IHeroModel SelectedHero
        {
            get => _selectedHero;
            set
            {
                if (SetProperty(ref _selectedHero, value) && _selectedHero != null)
                {
                    LoadHeroDetails(_selectedHero.Id);
                }
            }
        }

        // Gold property
        public float AvailableGold
        {
            get => _availableGold;
            private set => SetProperty(ref _availableGold, value);
        }

        // Inventory capacity property
        public int InventoryCapacity
        {
            get => _inventoryCapacity;
            private set => SetProperty(ref _inventoryCapacity, value);
        }

        // Inventory items collection property
        public ObservableCollection<IItemModel> InventoryItems
        {
            get => _inventoryItems;
            private set => SetProperty(ref _inventoryItems, value);
        }

        // Method to load all heroes from the model API
        private void LoadHeroes()
        {
            Heroes.Clear();

            var heroModels = _modelAPI.GetAllHeroes();
            if (heroModels != null)
            {
                foreach (var heroModel in heroModels)
                {
                    // We assume the ModelAPI converts models to DTOs internally
                    var heroDto = heroModel as IHeroModel;
                    if (heroDto != null)
                    {
                        Heroes.Add(heroDto);
                    }
                }
            }
        }

        // Method to load details of the selected hero
        private void LoadHeroDetails(Guid heroId)
        {
            var heroModel = _modelAPI.GetHero(heroId);
            if (heroModel != null)
            {
                var hero = heroModel as IHeroModel;
                if (hero != null)
                {
                    AvailableGold = hero.Gold;

                    // Load inventory items
                    InventoryItems.Clear();

                    if (hero.Inventory != null)
                    {
                        InventoryCapacity = hero.Inventory.Capacity;

                        foreach (var item in hero.Inventory.Items)
                        {
                            InventoryItems.Add(item as IItemModel);
                        }
                    }
                    else
                    {
                        InventoryCapacity = 0;
                    }
                }
            }
        }

        // Method to refresh the view model
        public void Refresh()
        {
            LoadHeroes();

            // Refresh selected hero details if one is selected
            if (SelectedHero != null)
            {
                LoadHeroDetails(SelectedHero.Id);
            }
        }
    }
}
