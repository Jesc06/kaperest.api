using KapeRest.Domain.Entities.InventoryEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.MenuEntities
{
    public class MenuItemProduct
    {
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

        public int ProductOfSupplierId { get; set; }
        public ProductOfSupplier ProductOfSupplier { get; set; }

        public int QuantityUsed { get; set; }
    }
}
