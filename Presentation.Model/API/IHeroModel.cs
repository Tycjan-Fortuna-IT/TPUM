namespace Presentation.Model.API
{
    public interface IHeroModel
    {
        public abstract Guid Id { get; }
        public abstract string Name { get; }
        public abstract float Gold { get; }
        public abstract IInventoryModel Inventory { get; }
    }
}