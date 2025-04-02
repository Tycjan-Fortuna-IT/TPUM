namespace Presentation.Model.API
{
    public interface IOrderModel
    {
        public abstract Guid Id { get; }
        public abstract IHeroModel Buyer { get; }
        public abstract IEnumerable<IItemModel> ItemsToBuy { get; }
    }
}