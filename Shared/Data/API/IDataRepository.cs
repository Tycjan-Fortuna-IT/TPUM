namespace Shared.Data.API
{
    public interface IDataRepository
    {
        public abstract IEnumerable<IHero> GetAllHeroes();
        public abstract IHero? GetHero(Guid id);
        public abstract void AddHero(IHero hero);
        public abstract bool RemoveHeroById(Guid id);
        public abstract bool RemoveHero(IHero hero);
        public abstract bool UpdateHero(Guid id, IHero hero);

        public abstract IEnumerable<IInventory> GetAllInventories();
        public abstract IInventory? GetInventory(Guid id);
        public abstract void AddInventory(IInventory inventory);
        public abstract bool RemoveInventoryById(Guid id);
        public abstract bool RemoveInventory(IInventory inventory);
        public abstract bool UpdateInventory(Guid id, IInventory inventory);

        public abstract IEnumerable<IItem> GetAllItems();
        public abstract IItem? GetItem(Guid id);
        public abstract void AddItem(IItem item);
        public abstract bool RemoveItemById(Guid id);
        public abstract bool RemoveItem(IItem item);
        public abstract bool UpdateItem(Guid id, IItem item);

        public abstract IEnumerable<IOrder> GetAllOrders();
        public abstract IOrder? GetOrder(Guid id);
        public abstract void AddOrder(IOrder order);
        public abstract bool RemoveOrderById(Guid id);
        public abstract bool RemoveOrder(IOrder order);
        public abstract bool UpdateOrder(Guid id, IOrder order);
    }
}
