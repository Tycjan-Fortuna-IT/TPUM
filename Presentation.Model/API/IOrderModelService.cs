namespace Presentation.Model.API
{
    public interface IOrderModelService
    {
        public abstract IEnumerable<IOrderModel> GetAllOrders();
        public abstract IOrderModel? GetOrder(Guid id);
        public abstract void AddOrder(Guid id, Guid buyerId, IEnumerable<Guid> itemIds);
        public abstract bool RemoveOrder(Guid id);

        public abstract void TriggerPeriodicOrderProcessing();
    }
}