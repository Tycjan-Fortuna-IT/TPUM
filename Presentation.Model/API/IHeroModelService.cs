namespace Presentation.Model.API
{
    public interface IHeroModelService
    {
        public abstract IEnumerable<IHeroModel> GetAllHeroes();
        public abstract IHeroModel? GetHero(Guid id);
        public abstract void AddHero(Guid id, string name, float gold, Guid inventoryId);
        public abstract bool RemoveHero(Guid id);
        public abstract bool UpdateHero(Guid id, string name, float gold, Guid inventoryId);
        public abstract void TriggerPeriodicItemMaintenanceDeduction();
    }
}