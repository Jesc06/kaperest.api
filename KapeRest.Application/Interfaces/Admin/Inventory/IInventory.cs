using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Application.DTOs.Admin.Inventory;


namespace KapeRest.Application.Interfaces.Admin.Inventory
{
    public interface IInventory
    {
        Task<ProductResponseDTO> AddProduct(CreateProductDTO addProduct);
        Task<SupplierResponseDTO> AddSupplier(CreateSupplierDTO addSupplier);
    }
}
