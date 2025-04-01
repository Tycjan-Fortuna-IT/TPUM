namespace Presentation.Model.API
{
    public interface IItemModel
    {
        Guid Id { get; }
        string Name { get; }
        int Price { get; }
        int MaintenanceCost { get; }
    }
}