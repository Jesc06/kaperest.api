using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.MenuEntities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }       // e.g., Milk Tea
        public decimal Price { get; set; }

        // Map products used for this item
        public ICollection<MenuItemProduct> Products { get; set; }
    }
}
