using System.Collections.Generic;

namespace Presentation.Model.API
{
    public interface IInventoryModel
    {
        Guid Id { get; }
        int Capacity { get; }
        IEnumerable<IItemModel> Items { get; }
    }
}
