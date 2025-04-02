
using Logic.API;

namespace Presentation.Model.Tests
{
    // Dummy implementation of IHeroLogic
    internal class DummyHeroLogic : IHeroLogic
    {
        internal readonly Dictionary<Guid, IHeroDataTransferObject> Heroes = new();
        public int PeriodicDeductionCallCount { get; private set; } = 0;

        public void Add(IHeroDataTransferObject item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Heroes[item.Id] = item;
        }

        public IEnumerable<IHeroDataTransferObject> GetAll()
        {
            return Heroes.Values.ToList();
        }

        public IHeroDataTransferObject? Get(Guid id)
        {
            Heroes.TryGetValue(id, out IHeroDataTransferObject? hero);
            return hero;
        }

        public void PeriodicItemMaintenanceDeduction()
        {
            PeriodicDeductionCallCount++;
        }

        public bool Remove(IHeroDataTransferObject item)
        {
            if (item == null) return false;
            return Heroes.Remove(item.Id);
        }

        public bool RemoveById(Guid id)
        {
            return Heroes.Remove(id);
        }

        public bool Update(Guid id, IHeroDataTransferObject item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (Heroes.ContainsKey(id))
            {
                Heroes[id] = item;
                return true;
            }
            return false;
        }
    }


    // Dummy implementation of IInventoryLogic
    internal class DummyInventoryLogic : IInventoryLogic
    {
        internal readonly Dictionary<Guid, IInventoryDataTransferObject> Inventories = new();

        public void Add(IInventoryDataTransferObject item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Inventories[item.Id] = item;
        }

        public IEnumerable<IInventoryDataTransferObject> GetAll()
        {
            return Inventories.Values.ToList();
        }

        public IInventoryDataTransferObject? Get(Guid id)
        {
            Inventories.TryGetValue(id, out IInventoryDataTransferObject? inventory);
            return inventory;
        }

        public bool Remove(IInventoryDataTransferObject item)
        {
            if (item == null) return false;
            return Inventories.Remove(item.Id);
        }

        public bool RemoveById(Guid id)
        {
            return Inventories.Remove(id);
        }

        public bool Update(Guid id, IInventoryDataTransferObject item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (Inventories.ContainsKey(id))
            {
                Inventories[id] = item;
                return true;
            }
            return false;
        }
    }

    // Dummy implementation of IItemLogic
    internal class DummyItemLogic : IItemLogic
    {
        internal readonly Dictionary<Guid, IItemDataTransferObject> Items = new();

        public void Add(IItemDataTransferObject item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Items[item.Id] = item;
        }

        public IEnumerable<IItemDataTransferObject> GetAll()
        {
            return Items.Values.ToList();
        }

        public IItemDataTransferObject? Get(Guid id)
        {
            Items.TryGetValue(id, out IItemDataTransferObject? item);
            return item;
        }

        public bool Remove(IItemDataTransferObject item)
        {
            if (item == null) return false;
            return Items.Remove(item.Id);
        }

        public bool RemoveById(Guid id)
        {
            return Items.Remove(id);
        }

        public bool Update(Guid id, IItemDataTransferObject item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (Items.ContainsKey(id))
            {
                Items[id] = item;
                return true;
            }
            return false;
        }
    }

    // Dummy implementation of IOrderLogic
    internal class DummyOrderLogic : IOrderLogic
    {
        internal readonly Dictionary<Guid, IOrderDataTransferObject> Orders = new();
        public int PeriodicProcessingCallCount { get; private set; } = 0;

        public void Add(IOrderDataTransferObject item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            Orders[item.Id] = item;
        }

        public IEnumerable<IOrderDataTransferObject> GetAll()
        {
            return Orders.Values.ToList();
        }

        public IOrderDataTransferObject? Get(Guid id)
        {
            Orders.TryGetValue(id, out IOrderDataTransferObject? order);
            return order;
        }

        public void PeriodicOrderProcessing()
        {
            PeriodicProcessingCallCount++;
        }

        public bool Remove(IOrderDataTransferObject item)
        {
            if (item == null) return false;
            return Orders.Remove(item.Id);
        }

        public bool RemoveById(Guid id)
        {
            return Orders.Remove(id);
        }

        public bool Update(Guid id, IOrderDataTransferObject item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (Orders.ContainsKey(id))
            {
                Orders[id] = item;
                return true;
            }
            return false;
        }
    }


    internal class ConcreteHeroDto : IHeroDataTransferObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Gold { get; set; }
        // Uses object reference as per the interface
        public IInventoryDataTransferObject Inventory { get; set; } = null!; // Initialize with null-forgiving, set in test setup
    }

    internal class ConcreteInventoryDto : IInventoryDataTransferObject
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        // Uses object references as per the interface
        public IEnumerable<IItemDataTransferObject> Items { get; set; } = Enumerable.Empty<IItemDataTransferObject>();
    }

    internal class ConcreteItemDto : IItemDataTransferObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public int MaintenanceCost { get; set; }
    }

    internal class ConcreteOrderDto : IOrderDataTransferObject
    {
        public Guid Id { get; set; }
        // Uses object reference as per the interface
        public IHeroDataTransferObject Buyer { get; set; } = null!; // Initialize with null-forgiving, set in test setup
        // Uses object references as per the interface
        public IEnumerable<IItemDataTransferObject> ItemsToBuy { get; set; } = Enumerable.Empty<IItemDataTransferObject>();
    }
}