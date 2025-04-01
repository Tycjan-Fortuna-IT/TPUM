using System.Collections.Generic;

namespace Presentation.Model.API
{
    public interface IOrderModelService
    {
        IEnumerable<IOrderModel> GetAllOrders();
        IOrderModel? GetOrder(Guid id);
        void AddOrder(Guid id, Guid buyerId, IEnumerable<Guid> itemIds);
        bool RemoveOrder(Guid id);

        void TriggerPeriodicOrderProcessing(); 
        void TriggerRestockItems();
    }
}