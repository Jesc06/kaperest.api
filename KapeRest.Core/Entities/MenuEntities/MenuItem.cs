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
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
<<<<<<< HEAD
        public string Image { get; set; }
=======
        public byte[] Image { get; set; }
>>>>>>> 046385f (updated)

        // Navigation Property
        public ICollection<MenuItemProduct> MenuItemProducts { get; set; } = new List<MenuItemProduct>();
    }
}
