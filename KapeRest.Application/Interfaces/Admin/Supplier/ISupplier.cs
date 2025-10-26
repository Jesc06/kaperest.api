using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.DTOs.Admin.Supplier;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Admin.Supplier
{
    public interface ISupplier
    {
        Task<SupplierResponseDTO> AddSupplier(CreateSupplierDTO addSupplier);
        Task<SupplierResponseDTO> UpdateSupplier(UpdateSupplierDTO update);
        Task<bool> DeleteSupplier(int productId);
        Task<ICollection> GetAllSupplier();
    }
}
