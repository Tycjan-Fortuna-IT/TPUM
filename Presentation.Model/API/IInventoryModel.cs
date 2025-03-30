using Data.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Model.API
{
    public interface IInventoryModel
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public List<IItemModel> Items { get; set; }
    }
}
