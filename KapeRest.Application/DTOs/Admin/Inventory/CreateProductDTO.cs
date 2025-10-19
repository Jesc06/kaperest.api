using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Admin.Inventory
{
    public class CreateProductDTO
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }

        //connect to Supplier
        public int SupplierId { get; set; }

        public string Base64Image { get; set; }  // frontend can send image as Base64
        public string ImageMimeType { get; set; }
    }
}
