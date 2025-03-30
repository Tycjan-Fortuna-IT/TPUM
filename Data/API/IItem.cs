namespace Data.API
{
    public interface IItem : IIdentifiable
    {
        public abstract string Name { get; }

        public abstract int Price { get; }

        public abstract int MaintenanceCost { get; }
    }
}
