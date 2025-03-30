using Presentation.Model.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Model.API
{
    public interface IHeroModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Gold { get; set; }
        public IInventoryModel Inventory { get; set; }
    }
}
