using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Admin.Inventory
{
    public class UpdateProductDTO
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public decimal? Prices { get; set; }
        public int? Stocks { get; set; }
        public string? Units { get; set; }
    }
}
