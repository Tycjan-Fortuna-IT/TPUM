using System.Collections.Generic;

namespace Presentation.Model.API
{
    public interface IOrderModel
    {
        Guid Id { get; }
        IHeroModel Buyer { get; }
        IEnumerable<IItemModel> ItemsToBuy { get; }
    }
}