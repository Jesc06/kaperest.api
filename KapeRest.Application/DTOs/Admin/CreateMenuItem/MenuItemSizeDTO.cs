using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Admin.CreateMenuItem
{
    public class MenuItemSizeDTO
    {
        public int? Id { get; set; }
        public string Size { get; set; } // Small, Medium, Large
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
