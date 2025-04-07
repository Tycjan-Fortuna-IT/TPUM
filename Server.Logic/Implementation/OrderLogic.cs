using Shared.Data.API;
using Shared.Logic.API;

namespace Server.Logic.Implementation
{
    internal class OrderLogic : IOrderLogic
    {
        private IDataRepository _repository;
        private readonly object _lock = new object();

        public OrderLogic(IDataRepository repository)
        {
            this._repository = repository;
        }

        public static IOrderDataTransferObject Map(IOrder order)
        {
            List<IItemDataTransferObject> mappedItems = new List<IItemDataTransferObject>();

            foreach (IItem item in order.ItemsToBuy)
            {
                mappedItems.Add(ItemLogic.Map(item));
            }

            return new OrderDataTransferObject(order.Id, HeroLogic.Map(order.Buyer), mappedItems);
        }

        public IEnumerable<IOrderDataTransferObject> GetAll()
        {
            lock (_lock)
            {
                List<IOrderDataTransferObject> all = new List<IOrderDataTransferObject>();

                foreach (IOrder order in _repository.GetAllOrders())
                {
                    all.Add(Map(order));
                }

                return all;
            }
        }

        public IOrderDataTransferObject? Get(Guid id)
        {
            lock (_lock)
            {
                IOrder? order = _repository.GetOrder(id);

                return order is not null ? Map(order) : null;
            }
        }

        public void Add(IOrderDataTransferObject order)
        {
            lock (_lock)
            {
                _repository.AddOrder(new MappedDataOrder(order));
            }
        }

        public bool RemoveById(Guid id)
        {
            lock (_lock)
            {
                return _repository.RemoveOrderById(id);
            }
        }

        public bool Remove(IOrderDataTransferObject order)
        {
            lock (_lock)
            {
                return _repository.RemoveOrder(new MappedDataOrder(order));
            }
        }

        public bool Update(Guid id, IOrderDataTransferObject order)
        {
            lock (_lock)
            {
                return _repository.UpdateOrder(id, new MappedDataOrder(order));
            }
        }

        public void PeriodicOrderProcessing()
        {
            lock (_lock)
            {
                foreach (IOrderDataTransferObject order in GetAll())
                {
                    IHeroDataTransferObject buyer = order.Buyer;

                    List<IItemDataTransferObject> newInventoryItems = new List<IItemDataTransferObject>();
                    foreach (IItemDataTransferObject item in order.ItemsToBuy)
                    {
                        newInventoryItems.Add(item);

                        buyer.Gold -= item.Price;

                        _repository.RemoveItem(new MappedDataItem(item));
                    }

                    foreach (IItemDataTransferObject item in buyer.Inventory.Items)
                    {
                        newInventoryItems.Add(item);
                    }

                    buyer.Inventory = new InventoryDataTransferObject(buyer.Inventory.Id, buyer.Inventory.Capacity, newInventoryItems);

                    _repository.UpdateHero(buyer.Id, new MappedDataHero(buyer));

                    _repository.RemoveOrder(new MappedDataOrder(order));
                }
            }
        }
    }
}
