namespace Presentation.Model.API
{
    public interface IHeroModel
    {
        Guid Id { get; }
        string Name { get; }
        float Gold { get; }
        IInventoryModel Inventory { get; }
    }
}