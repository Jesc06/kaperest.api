    using KapeRest.Core.Entities.MenuEntities;
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
            public decimal Price { get; set; } // Base price (can be used as default or Small price)
            public string Category { get; set; }
            public string Description { get; set; }
            public string IsAvailable { get; set; }   
            public byte[]? Image { get; set; }
            public string CashierId { get; set; } // link to cashier
            public int? BranchId { get; set; }    // optional branch

            public ICollection<MenuItemProduct> MenuItemProducts { get; set; }
            public ICollection<MenuItemSize> MenuItemSizes { get; set; }
            
            public MenuItem()
            {
                MenuItemProducts = new List<MenuItemProduct>();
                MenuItemSizes = new List<MenuItemSize>();
            }
        }
    }
