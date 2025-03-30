using Presentation.Model.API;
using Presentation.ViewModel.MVVMLight;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.ViewModel
{
    public class InventoryViewModel : ViewModelBase
    {
        private readonly ModelAbstractAPI _modelAPI;
        private ObservableCollection<InventoryItemViewModel> _inventoryItems;
        private InventoryItemViewModel? _selectedInventoryItem;
        private Guid _currentHeroId;

        public InventoryViewModel(ModelAbstractAPI modelAPI)
        {
            _modelAPI = modelAPI ?? throw new ArgumentNullException(nameof(modelAPI));
            _inventoryItems = new ObservableCollection<InventoryItemViewModel>();
            _currentHeroId = Guid.Empty;
        }

        public ObservableCollection<InventoryItemViewModel> InventoryItems
        {
            get => _inventoryItems;
            set => SetProperty(ref _inventoryItems, value);
        }

        public InventoryItemViewModel? SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set => SetProperty(ref _selectedInventoryItem, value);
        }

        public Guid CurrentHeroId
        {
            get => _currentHeroId;
            set
            {
                if (SetProperty(ref _currentHeroId, value))
                {
                    LoadHeroInventory(_currentHeroId);
                }
            }
        }

        public void LoadHeroInventory(Guid heroId)
        {
            _currentHeroId = heroId;
            var inventories = _modelAPI.GetAllInventories().Where(i => i.HeroId == heroId);

            var items = new ObservableCollection<InventoryItemViewModel>();
            foreach (var inventory in inventories)
            {
                var item = _modelAPI.GetItem(inventory.ItemId);
                if (item != null)
                {
                    items.Add(new InventoryItemViewModel
                    {
                        InventoryId = inventory.Id,
                        ItemId = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Value = item.Value,
                        Quantity = inventory.Quantity
                    });
                }
            }

            InventoryItems = items;
        }

        public void RefreshInventory()
        {
            if (_currentHeroId != Guid.Empty)
            {
                LoadHeroInventory(_currentHeroId);
            }
        }
    }

    public class InventoryItemViewModel : ViewModelBase
    {
        private Guid _inventoryId;
        private Guid _itemId;
        private string _name;
        private string _description;
        private decimal _value;
        private int _quantity;

        public Guid InventoryId
        {
            get => _inventoryId;
            set => SetProperty(ref _inventoryId, value);
        }

        public Guid ItemId
        {
            get => _itemId;
            set => SetProperty(ref _itemId, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public decimal Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }
    }
}
