using Data.API;
using Logic.API;

namespace Logic.Implementation
{
    internal class OrderLogic : IOrderLogic
    {
        private IDataRepository _repository;

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
            List<IOrderDataTransferObject> all = new List<IOrderDataTransferObject>();

            foreach (IOrder order in _repository.GetAllOrders())
            {
                all.Add(Map(order));
            }

            return all;
        }

        public IOrderDataTransferObject? Get(Guid id)
        {
            IOrder? order = _repository.GetOrder(id);

            return order is not null ? Map(order) : null;
        }

        public void Add(IOrderDataTransferObject order)
        {
            _repository.AddOrder(new MappedDataOrder(order));
        }

        public bool RemoveById(Guid id)
        {
            return _repository.RemoveOrderById(id);
        }

        public bool Remove(IOrderDataTransferObject order)
        {
            return _repository.RemoveOrder(new MappedDataOrder(order));
        }

        public bool Update(Guid id, IOrderDataTransferObject order)
        {
            return _repository.UpdateOrder(id, new MappedDataOrder(order));
        }

        public void PeriodicOrderProcessing()
        {
            // TODO
        }

        public void RestockItems()
        {
            // TODO
        }
    }
}
