using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Domain.Entities.MenuEntities;

namespace KapeRest.Core.Entities.MenuEntities
{
    public class MenuItemSize
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string Size { get; set; } // Small, Medium, Large
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;

        // Navigation property
        public MenuItem MenuItem { get; set; }
    }
}
