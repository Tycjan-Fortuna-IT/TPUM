using Presentation.Model.API;
using Logic.API; // Uses Logic API
using System;
using System.Collections.Generic;
using System.Linq;
using Presentation.Model.Implementation.Transient; // For helper DTOs

namespace Presentation.Model.Implementation
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
            var dto = _itemLogic.Get(id);
            return dto == null ? null : new ItemModel(dto); // Map DTO to Model
        }

        public void AddItem(Guid id, string name, int price, int maintenanceCost)
        {
            // Create a transient DTO to pass to the logic layer
            var transientDto = new TransientItemDTO(id, name, price, maintenanceCost);
            _itemLogic.Add(transientDto);
        }

        public bool RemoveItem(Guid id)
        {
            return _itemLogic.RemoveById(id);
        }

        public bool UpdateItem(Guid id, string name, int price, int maintenanceCost)
        {
            // Create a transient DTO to pass to the logic layer
            var transientDto = new TransientItemDTO(id, name, price, maintenanceCost);
            return _itemLogic.Update(id, transientDto);
        }
    }
}
