namespace Shared.Logic.API
{
    public interface IOrderDataTransferObject
    {
        public abstract Guid Id { get; }

        public abstract IHeroDataTransferObject Buyer { get; }

        public abstract IEnumerable<IItemDataTransferObject> ItemsToBuy { get; }
    }
}
