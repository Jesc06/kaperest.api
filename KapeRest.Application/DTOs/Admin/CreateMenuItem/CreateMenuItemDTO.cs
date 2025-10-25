using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Admin.CreateMenuItem
{
    public class CreateMenuItemDTO
    {
        public string Item_name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public List<MenuItemProductDTO> Products { get; set; } = new();
    }
}
