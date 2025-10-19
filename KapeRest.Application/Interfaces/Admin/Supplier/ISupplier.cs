using KapeRest.Application.DTOs.Admin.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Admin.Supplier
{
    public interface ISupplier
    {
        Task<SupplierResponseDTO> AddSupplier(CreateSupplierDTO addSupplier);
    }
}
