using Logic.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Model.API
{
    public abstract class ModelAbstractAPI
    {
        public abstract IEnumerable<IHeroModel> GetAllHeroes();
        public abstract IHeroModel? GetHero(Guid id);
        public abstract void AddHero(IHeroModel hero);
        public abstract bool RemoveHero(Guid id);
        public abstract bool UpdateHero(Guid id, IHeroModel hero);
        public abstract void PerformHeroMaintenance();

        // Inventory-related operations
        public abstract IEnumerable<IInventoryModel> GetAllInventories();
        public abstract IInventoryModel? GetInventory(Guid id);
        public abstract void AddInventory(IInventoryModel inventory);
        public abstract bool RemoveInventory(Guid id);
        public abstract bool UpdateInventory(Guid id, IInventoryModel inventory);

        // Item-related operations
        public abstract IEnumerable<IItemModel> GetAllItems();
        public abstract IItemModel? GetItem(Guid id);
        public abstract void AddItem(IItemModel item);
        public abstract bool RemoveItem(Guid id);
        public abstract bool UpdateItem(Guid id, IItemModel item);

        // Order-related operations
        public abstract IEnumerable<IOrderModel> GetAllOrders();
        public abstract IOrderModel? GetOrder(Guid id);
        public abstract void AddOrder(IOrderModel order);
        public abstract bool RemoveOrder(Guid id);
        public abstract bool UpdateOrder(Guid id, IOrderModel order);
        public abstract void ProcessOrders();
        public abstract void RestockItems();

        public static ModelAPIImplementation CreateApi(
        IHeroLogic heroLogic = null,
        IInventoryLogic inventoryLogic = null,
        IItemLogic itemLogic = null,
        IOrderLogic orderLogic = null)
        {
            return new ModelAPIImplementation(
                heroLogic ?? LogicFactory.CreateHeroLogic(),
                inventoryLogic ?? LogicFactory.CreateInventoryLogic(),
                itemLogic ?? LogicFactory.CreateItemLogic(),
                orderLogic ?? LogicFactory.CreateOrderLogic(),
                IDTOMapper.CreateMapper()
            );
        }
    }
}
