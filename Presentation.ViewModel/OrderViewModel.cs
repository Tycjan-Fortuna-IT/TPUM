using Presentation.Model.API;
using Presentation.ViewModel.MVVMLight;
using System.Collections.ObjectModel;

namespace Presentation.ViewModel
{
    public class OrderViewModel : ViewModelBase
    {
        private readonly ModelAbstractAPI _modelAPI;

        private IHeroModel _buyer;
        private ObservableCollection<IItemModel> _itemsToBuy;
        private float _totalCost;
        private IItemModel _selectedOrderItem;
        private string _orderStatus;
        private bool _canPlaceOrder;

        public OrderViewModel(ModelAbstractAPI modelAPI)
        {
            _modelAPI = modelAPI ?? throw new ArgumentNullException(nameof(modelAPI));

            // Initialize collections
            ItemsToBuy = new ObservableCollection<IItemModel>();
            UpdateOrderStatus();
        }

        // Buyer property
        public IHeroModel Buyer
        {
            get => _buyer;
            set
            {
                if (SetProperty(ref _buyer, value))
                {
                    UpdateOrderStatus();
                    CheckCanPlaceOrder();
                }
            }
        }

        // Items to buy collection property
        public ObservableCollection<IItemModel> ItemsToBuy
        {
            get => _itemsToBuy;
            private set => SetProperty(ref _itemsToBuy, value);
        }

        // Total cost property
        public float TotalCost
        {
            get => _totalCost;
            private set => SetProperty(ref _totalCost, value);
        }

        // Selected order item property
        public IItemModel SelectedOrderItem
        {
            get => _selectedOrderItem;
            set => SetProperty(ref _selectedOrderItem, value);
        }

        // Order status property
        public string OrderStatus
        {
            get => _orderStatus;
            private set => SetProperty(ref _orderStatus, value);
        }

        // Can place order property
        public bool CanPlaceOrder
        {
            get => _canPlaceOrder;
            private set => SetProperty(ref _canPlaceOrder, value);
        }

        // Method to add an item to the order
        public void AddItemToOrder(IItemModel item)
        {
            if (item == null)
                return;

            ItemsToBuy.Add(item);
            CalculateTotalCost();
            UpdateOrderStatus();
            CheckCanPlaceOrder();
        }

        // Method to remove the selected item from the order
        public void RemoveSelectedItemFromOrder()
        {
            if (SelectedOrderItem == null)
                return;

            ItemsToBuy.Remove(SelectedOrderItem);
            SelectedOrderItem = null;
            CalculateTotalCost();
            UpdateOrderStatus();
            CheckCanPlaceOrder();
        }

        // Method to clear all items from the order
        public void ClearOrder()
        {
            ItemsToBuy.Clear();
            SelectedOrderItem = null;
            CalculateTotalCost();
            UpdateOrderStatus();
            CheckCanPlaceOrder();
        }

        // Method to calculate the total cost of all items in the order
        private void CalculateTotalCost()
        {
            TotalCost = ItemsToBuy.Sum(item => item.Price);
        }

        // Method to update the order status text
        private void UpdateOrderStatus()
        {
            if (Buyer == null)
            {
                OrderStatus = "No hero selected";
                return;
            }

            if (ItemsToBuy.Count == 0)
            {
                OrderStatus = "Cart is empty";
                return;
            }

            float remainingGold = Buyer.Gold - TotalCost;
            if (remainingGold >= 0)
            {
                OrderStatus = $"Total: {TotalCost} gold. You will have {remainingGold} gold remaining.";
            }
            else
            {
                OrderStatus = $"Total: {TotalCost} gold. You need {Math.Abs(remainingGold)} more gold!";
            }
        }

        // Method to check if the order can be placed
        private void CheckCanPlaceOrder()
        {
            CanPlaceOrder = (Buyer != null && ItemsToBuy.Count > 0 && Buyer.Gold >= TotalCost);
        }

        // Method to place the order
        public bool PlaceOrder()
        {
            if (!CanPlaceOrder)
                return false;

            if (Buyer == null || Buyer.Inventory == null)
                return false;

            // Check if the hero's inventory has enough space
            var hero = _modelAPI.GetHero(Buyer.Id) as IHeroModel;
            if (hero == null || hero.Inventory == null)
                return false;

            int currentItems = hero.Inventory.Items.Count();
            int capacity = hero.Inventory.Capacity;

            if (currentItems + ItemsToBuy.Count > capacity)
            {
                OrderStatus = $"Not enough inventory space! Capacity: {capacity}, Current: {currentItems}, Adding: {ItemsToBuy.Count}";
                return false;
            }

            // Create a new order
            var orderDto = IOrderModel.CreateOrder(hero as IHeroModel, ItemsToBuy.Cast<IItemModel>().ToList());

            // Add the order
            _modelAPI.AddOrder(orderDto);

            // Process orders
            _modelAPI.ProcessOrders();

            // Clear the order
            ClearOrder();
            OrderStatus = "Order placed successfully!";

            return true;
        }
    }
}
