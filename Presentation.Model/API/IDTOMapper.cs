using Logic.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Model.API
{
    public interface IDTOMapper
    {
        IHeroModel MapToHeroModel(IHeroDataTransferObject dto);
        IHeroDataTransferObject MapToHeroDTO(IHeroModel model);

        IInventoryModel MapToInventoryModel(IInventoryDataTransferObject dto);
        IInventoryDataTransferObject MapToInventoryDTO(IInventoryModel model);

        IItemModel MapToItemModel(IItemDataTransferObject dto);
        IItemDataTransferObject MapToItemDTO(IItemModel model);

        IOrderModel MapToOrderModel(IOrderDataTransferObject dto);
        IOrderDataTransferObject MapToOrderDTO(IOrderModel model);

        public static IDTOMapper CreateMapper()
        {
            return new Model.Implementation.DTOMapper();
        }

    }
}
