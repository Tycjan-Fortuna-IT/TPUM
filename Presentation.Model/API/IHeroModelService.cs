using System.Collections.Generic;

namespace Presentation.Model.API
{
    public interface IHeroModelService
    {
        IEnumerable<IHeroModel> GetAllHeroes();
        IHeroModel? GetHero(Guid id);
        void AddHero(Guid id, string name, float gold, Guid inventoryId);
        bool RemoveHero(Guid id);
        bool UpdateHero(Guid id, string name, float gold, Guid inventoryId);
        void TriggerPeriodicItemMaintenanceDeduction();
    }
}