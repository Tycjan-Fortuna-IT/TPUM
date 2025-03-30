namespace Data.API
{
    public interface IOrder : IIdentifiable
    {
        public abstract IHero Buyer { get; }

        public abstract IEnumerable<IItem> ItemsToBuy { get; }
    }
}
