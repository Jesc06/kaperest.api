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
        public string ItemName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ICollection<MenuItemProduct> MenuItemProducts { get; set; } = new List<MenuItemProduct>();
    }
}
