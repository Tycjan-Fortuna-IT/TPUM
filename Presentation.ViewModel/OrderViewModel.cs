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
    public class OrderViewModel : ViewModelBase
    {
        private readonly ModelAbstractAPI _modelAPI;
        private Guid _currentHeroId;
        private ObservableCollection<IOrderModel> _orders;

        public OrderViewModel(ModelAbstractAPI modelAPI)
        {
            _modelAPI = modelAPI ?? throw new ArgumentNullException(nameof(modelAPI));
            _orders = new ObservableCollection<IOrderModel>();
            _currentHeroId = Guid.Empty;
        }

        public ObservableCollection<IOrderModel> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        public Guid CurrentHeroId
        {
            get => _currentHeroId;
            set
            {
                if (SetProperty(ref _currentHeroId, value))
                {
                    LoadHeroOrders(_currentHeroId);
                }
            }
        }

        public void LoadHeroOrders(Guid heroId)
        {
            var orders = _modelAPI.GetAllOrders().Where(o => o.HeroId == heroId);
            Orders = new ObservableCollection<IOrderModel>(orders);
        }

        public void CreateOrder(Guid itemId, int quantity)
        {
            if (CurrentHeroId == Guid.Empty)
                return;

            // Get hero to check balance
            var hero = _modelAPI.GetHero(CurrentHeroId);
            if (hero == null)
                return;

            // Create and add new order
            var order = new OrderModel
            {
                Id = Guid.NewGuid(),
                HeroId = CurrentHeroId,
                ItemId = itemId,
                Quantity = quantity,
                OrderDate = DateTime.Now,
                Status = "Pending"
            };

            _modelAPI.AddOrder(order);

            // Reload orders
            LoadHeroOrders(CurrentHeroId);

            // Notify that there may be changes to hero gold balance
            OnPropertyChanged(nameof(CurrentHeroId));
        }

        public void ProcessOrders()
        {
            _modelAPI.ProcessOrders();
            if (CurrentHeroId != Guid.Empty)
            {
                LoadHeroOrders(CurrentHeroId);
            }
        }
    }
