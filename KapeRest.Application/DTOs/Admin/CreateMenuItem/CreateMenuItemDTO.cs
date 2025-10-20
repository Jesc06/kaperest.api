using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Admin.CreateMenuItem
{
    public class CreateMenuItemDTO
    {
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public List<MenuItemIngredientDTO> Ingredients { get; set; }
    }
}
