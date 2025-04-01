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

        // requires mapping DTOs within implementation
        // void AddHero(IHeroModel hero);
        // bool RemoveHero(IHeroModel hero);
        // bool UpdateHero(Guid id, IHeroModel hero);

        void TriggerPeriodicItemMaintenanceDeduction();
    }
}