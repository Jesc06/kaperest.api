using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Admin.CreateMenuItem
{
    public class CreateMenuItemDTO
    {
        public string cashierId { get; set; }
        public string Item_name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public string IsAvailable { get; set; } = "Yes";
        public List<MenuItemProductDTO> Products { get; set; } = new();
    }
}
