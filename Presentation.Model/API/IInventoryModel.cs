namespace Presentation.Model.API
{
    public interface IInventoryModel
    {
        public abstract Guid Id { get; }
        public abstract int Capacity { get; }
        public abstract IEnumerable<IItemModel> Items { get; }
    }
}
