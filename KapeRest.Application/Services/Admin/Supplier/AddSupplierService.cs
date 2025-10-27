using KapeRest.Application.DTOs.Admin.Supplier;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Supplier;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.Supplier
{
    public class AddSupplierService
    {
        private readonly ISupplier _supplier;
        public AddSupplierService(ISupplier supplier)
        {
            _supplier = supplier;
        }
        public async Task<SupplierResponseDTO> addSupplier(CreateSupplierDTO add)
        {
            return await _supplier.AddSupplier(add);
        }

        public async Task<SupplierResponseDTO> UpdateSupplier(UpdateSupplierDTO update)
        {
            return await _supplier.UpdateSupplier(update);
        }

        public async Task<bool> DeleteSupplier(int supplierId)
        {
            return await _supplier.DeleteSupplier(supplierId);
        }

        public async Task<ICollection> GetAllSuppliers()
        {
            return await _supplier.GetAllSupplier();
        }

    }
}
