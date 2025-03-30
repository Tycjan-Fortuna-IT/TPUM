namespace Data.API
{
    public interface IHero : IIdentifiable
    {
        public abstract string Name { get; }

        public abstract float Gold { get; }

        public abstract IInventory Inventory { get; }
    }
}
