using Logic.API;
using Presentation.Model.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Model.Implementation
{
    public class DTOMapper : IDTOMapper
    {
        // ZASYPIAM JUŻ AAAAAAAAAAA dobra jutro to się zrobi trzeba lepsze mapowanie wymyślić albo pozmieniać
        // struktury tych klas sam już nie wiem
        public IHeroModel MapToHeroModel(IHeroDataTransferObject dto)
        {
            return new HeroModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Gold = dto.Gold,
                Inventory = MapToInventoryModel(dto.Inventory)
            };
        }

        public IHeroDataTransferObject MapToHeroDTO(IHeroModel model)
        {
            return new HeroDTO
            {
                Id = model.Id,
                Name = model.Name,
                Gold = model.Gold,
                Inventory = model.Inventory
            };  
        }

        public IInventoryModel MapToInventoryModel(IInventoryDataTransferObject dto)
        {
            return new InventoryModel
            {
                Id = dto.Id,
                Capacity = dto.Capacity,
                Items = dto.Items?.Select(MapToItemModel).ToList() ?? new List<IItemModel>()
            };
        }

        public IInventoryDataTransferObject MapToInventoryDTO(IInventoryModel model)
        {
            return new InventoryDTO
            {
                Id = model.Id,
                Capacity = model.Capacity,
                Items = model.Items?.Select(MapToItemDTO).ToList() ?? new List<IItemDataTransferObject>()
            };
        }

        public IItemModel MapToItemModel(IItemDataTransferObject dto)
        {
            return new ItemModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                MaintenanceCost = dto.MaintenanceCost
            };
        }

        public IItemDataTransferObject MapToItemDTO(IItemModel model)
        {
            return new ItemDTO
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                MaintenanceCost = model.MaintenanceCost
            };
        }

        public IOrderModel MapToOrderModel(IOrderDataTransferObject dto)
        {
            return new OrderModel
            {
                Id = dto.Id,
                Buyer = MapToHeroModel(dto.Buyer),
                ItemsToBuy = dto.ItemsToBuy // mapp all?
            };
        }

        public IOrderDataTransferObject MapToOrderDTO(IOrderModel model)
        {
            return new OrderDTO
            {
                Id = model.Id,
                Buyer = MapToHeroDTO(model.Buyer),
                ItemIds = model.ItemsToBuy?.Select(item => item.Id).ToList() ?? new List<Guid>()
            };
        }

        // Placeholder concrete implementations for DTOs
        private class HeroDTO : IHeroDataTransferObject
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public float Gold { get; set; }
            public List<IItemDataTransferObject> Inventory { get; set; } = new List<IItemDataTransferObject>();

            IInventoryDataTransferObject IHeroDataTransferObject.Inventory => throw new NotImplementedException();
        }

        private class InventoryDTO : IInventoryDataTransferObject
        {
            public Guid Id { get; set; }
            public int Capacity { get; set; }
            public List<IItemDataTransferObject> Items { get; set; } = new List<IItemDataTransferObject>();

            IEnumerable<IItemDataTransferObject> IInventoryDataTransferObject.Items => Items;
        }

        private class ItemDTO : IItemDataTransferObject
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
            public int MaintenanceCost { get; set; }
        }

        private class OrderDTO : IOrderDataTransferObject
        {
            public Guid Id { get; set; }
            public HeroDTO Buyer { get; set; }
            public List<IItemDataTransferObject> ItemsToBuy { get; set; } = new List<IItemDataTransferObject>();

            IEnumerable<IItemDataTransferObject> IOrderDataTransferObject.ItemsToBuy => ItemsToBuy;
            IHeroDataTransferObject IOrderDataTransferObject.Buyer => Buyer;
        }
    }
}
