using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.Supplier
{
    public class SupplierService
    {
        private readonly ISupplier _supplier;
        public SupplierService(ISupplier supplier)
        {
            _supplier = supplier;
        }
        public async Task<SupplierResponseDTO> addSupplier(CreateSupplierDTO add)
        {
            return await _supplier.AddSupplier(add);
        }
    }
}
