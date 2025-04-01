using Presentation.Model.API;
using Logic.API;
using System;
using System.Collections.Generic;
using System.Linq;
using Presentation.Model.Implementation.Transient;

namespace Presentation.Model.Implementation
{
    internal class HeroModelService : IHeroModelService
    {
        private readonly IHeroLogic _heroLogic;
        private readonly IInventoryLogic _inventoryLogic; // Might need this to fetch inventory for Add/Update

        public HeroModelService(IHeroLogic heroLogic, IInventoryLogic inventoryLogic)
        {
            _heroLogic = heroLogic ?? throw new ArgumentNullException(nameof(heroLogic));
            _inventoryLogic = inventoryLogic ?? throw new ArgumentNullException(nameof(inventoryLogic));
        }

        public IEnumerable<IHeroModel> GetAllHeroes()
        {
            return _heroLogic.GetAll()
                            .Select(dto => new HeroModel(dto)); // Map DTO to Model
        }

        public IHeroModel? GetHero(Guid id)
        {
            var dto = _heroLogic.Get(id);
            return dto == null ? null : new HeroModel(dto); // Map DTO to Model
        }

        public void AddHero(Guid id, string name, float gold, Guid inventoryId)
        {
            // We need an IInventoryDataTransferObject. Fetch it using the Logic Layer.
            var inventoryDto = _inventoryLogic.Get(inventoryId);
            if (inventoryDto == null)
            {
                // Or create a default one if applicable, depends on logic rules
                throw new InvalidOperationException($"Inventory with ID {inventoryId} not found.");
            }

            var transientDto = new TransientHeroDTO(id, name, gold, inventoryDto);
            _heroLogic.Add(transientDto);
        }


        public bool RemoveHero(Guid id)
        {
            return _heroLogic.RemoveById(id);
        }

        public bool UpdateHero(Guid id, string name, float gold, Guid inventoryId)
        {
            var inventoryDto = _inventoryLogic.Get(inventoryId);
            if (inventoryDto == null)
            {
                throw new InvalidOperationException($"Inventory with ID {inventoryId} not found for update.");
            }
            var transientDto = new TransientHeroDTO(id, name, gold, inventoryDto);
            return _heroLogic.Update(id, transientDto);
        }

        public void TriggerPeriodicItemMaintenanceDeduction()
        {
            _heroLogic.PeriodicItemMaintenanceDeduction();
        }
    }
}
