using Client.Data.API;
using ClientServer.Shared.Data.API;
using System;
using System.Collections.Generic;

namespace Client.Data.Tests
{
    internal class MockDataContext : IDataContext
    {
        private readonly Dictionary<Guid, IHero> _heroes = new Dictionary<Guid, IHero>();
        private readonly Dictionary<Guid, IItem> _items = new Dictionary<Guid, IItem>();
        private readonly Dictionary<Guid, IInventory> _inventories = new Dictionary<Guid, IInventory>();
        private readonly Dictionary<Guid, IOrder> _orders = new Dictionary<Guid, IOrder>();

        public Dictionary<Guid, IHero> Heroes => _heroes;
        public Dictionary<Guid, IItem> Items => _items;
        public Dictionary<Guid, IInventory> Inventories => _inventories;
        public Dictionary<Guid, IOrder> Orders => _orders;

        public event Action OnDataChanged = delegate { };

        
        public void TriggerDataChanged()
        {
            OnDataChanged?.Invoke(); //simulate the data changing in the context
        }

        public void ClearAll()
        {
            _heroes.Clear();
            _items.Clear();
            _inventories.Clear();
            _orders.Clear();
        }
    }
}