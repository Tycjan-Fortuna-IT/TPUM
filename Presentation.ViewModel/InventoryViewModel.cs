using Presentation.Model.API;
using Presentation.ViewModel.MVVMLight;
using System.Collections.ObjectModel;

namespace Presentation.ViewModel
{
    public class InventoryViewModel : ViewModelBase
    {
        private readonly ModelAbstractAPI _modelAPI;

        private Guid _heroId;
        private IInventoryModel _inventory;
        private ObservableCollection<IItemModel> _items;
        private int _capacity;
        private int _itemCount;
        private IItemModel _selectedItem;
        private string _inventoryStatus;

        public InventoryViewModel(ModelAbstractAPI modelAPI)
        {
            _modelAPI = modelAPI ?? throw new ArgumentNullException(nameof(modelAPI));

            // Initialize collections
            Items = new ObservableCollection<IItemModel>();
        }

        // Hero ID property
        public Guid HeroId
        {
            get => _heroId;
            set
            {
                if (SetProperty(ref _heroId, value) && _heroId != Guid.Empty)
                {
                    LoadInventory();
                }
            }
        }

        // Inventory property
        public IInventoryModel Inventory
        {
            get => _inventory;
            private set => SetProperty(ref _inventory, value);
        }

        // Items collection property
        public ObservableCollection<IItemModel> Items
        {
            get => _items;
            private set => SetProperty(ref _items, value);
        }

        // Capacity property
        public int Capacity
        {
            get => _capacity;
            private set => SetProperty(ref _capacity, value);
        }

        // Item count property
        public int ItemCount
        {
            get => _itemCount;
            private set => SetProperty(ref _itemCount, value);
        }

        // Selected item property
        public IItemModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        // Inventory status property (e.g. "5/10 items")
        public string InventoryStatus
        {
            get => _inventoryStatus;
            private set => SetProperty(ref _inventoryStatus, value);
        }

        // Method to load inventory for the specified hero
        public void LoadInventory()
        {
            if (_heroId == Guid.Empty)
                return;

            var hero = _modelAPI.GetHero(_heroId);
            if (hero == null)
                return;

            var heroDto = hero as IHeroModel;
            if (heroDto?.Inventory == null)
                return;

            Inventory = heroDto.Inventory;
            Capacity = Inventory.Capacity;

            // Load inventory items
            Items.Clear();
            foreach (var item in Inventory.Items)
            {
                Items.Add(item as IItemModel);
            }

            ItemCount = Items.Count;
            UpdateInventoryStatus();
        }

        // Update the inventory status text
        private void UpdateInventoryStatus()
        {
            InventoryStatus = $"{ItemCount}/{Capacity} items";
        }

        // Method to add an item to the inventory
        public bool AddItem(IItemModel item)
        {
            if (ItemCount >= Capacity)
                return false;

            if (item == null)
                return false;

            // Get the hero and inventory
            var hero = _modelAPI.GetHero(_heroId) as IHeroModel;
            if (hero == null || hero.Inventory == null)
                return false;

            // Add the item to the inventory
            // Note: This assumes the ModelAPI will handle the actual update to the data store
            var inventoryModel = _modelAPI.GetInventory(hero.Inventory.Id);
            if (inventoryModel == null)
                return false;

            // Add the item to the collection
            Items.Add(item);
            ItemCount = Items.Count;
            UpdateInventoryStatus();

            return true;
        }

        // Method to remove the selected item from the inventory
        public bool RemoveSelectedItem()
        {
            if (SelectedItem == null)
                return false;

            // Get the hero and inventory
            var hero = _modelAPI.GetHero(_heroId) as IHeroModel;
            if (hero == null || hero.Inventory == null)
                return false;

            // Remove the item from the inventory
            // Note: This assumes the ModelAPI will handle the actual update to the data store
            var inventoryModel = _modelAPI.GetInventory(hero.Inventory.Id);
            if (inventoryModel == null)
                return false;

            // Remove the item from the collection
            Items.Remove(SelectedItem);
            SelectedItem = null;
            ItemCount = Items.Count;
            UpdateInventoryStatus();

            return true;
        }

        // Method to check if the inventory has space for more items
        public bool HasSpace()
        {
            return ItemCount < Capacity;
        }

        // Method to refresh the view model
        public void Refresh()
        {
            if (_heroId != Guid.Empty)
            {
                LoadInventory();
            }
        }
    }
}
