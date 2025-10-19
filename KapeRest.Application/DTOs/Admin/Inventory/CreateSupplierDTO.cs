using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Admin.Inventory
{
    public class CreateSupplierDTO
    {
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
    }
}
