﻿using Client.Presentation.Model.API;
using ClientServer.Shared.Logic.API;

namespace Client.Presentation.Model.Implementation
{
    internal class ItemModelService : IItemModelService
    {
        private readonly IItemLogic _itemLogic;

        // Inject Logic API
        public ItemModelService(IItemLogic itemLogic)
        {
            _itemLogic = itemLogic ?? throw new ArgumentNullException(nameof(itemLogic));
        }

        public IEnumerable<IItemModel> GetAllItems()
        {
            return _itemLogic.GetAll()
                             .Select(dto => new ItemModel(dto)); // Map DTO to Model
        }

        public IItemModel? GetItem(Guid id)
        {
            IItemDataTransferObject? dto = _itemLogic.Get(id);
            return dto == null ? null : new ItemModel(dto); // Map DTO to Model
        }

        public void AddItem(Guid id, string name, int price, int maintenanceCost)
        {
            // Create Model DTO to pass to the logic layer
            TransientItemDTO modelDto = new TransientItemDTO(id, name, price, maintenanceCost);
            _itemLogic.Add(modelDto);
        }

        public bool RemoveItem(Guid id)
        {
            return _itemLogic.RemoveById(id);
        }

        public bool UpdateItem(Guid id, string name, int price, int maintenanceCost)
        {
            // Create a Model DTO to pass to the logic layer
            TransientItemDTO modelDto = new TransientItemDTO(id, name, price, maintenanceCost);
            return _itemLogic.Update(id, modelDto);
        }
    }
}
