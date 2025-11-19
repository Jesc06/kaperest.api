using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Admin.Inventory
{
    public class CreateProductDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal CostPrice { get; set; }
        public int Stocks { get; set; }
        public string Units { get; set; }

        //connect to Supplier
        public int SupplierId { get; set; }
        public string CashierId { get; set; } = string.Empty;
        public int? BranchId { get; set; }
    }
}
