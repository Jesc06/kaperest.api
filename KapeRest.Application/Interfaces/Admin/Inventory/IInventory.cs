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
        Task<ProductResponseDTO> AddProductOfSuppliers(string currentUser,string role,CreateProductDTO addProduct);
        Task<ProductResponseDTO> UpdateProductOfSuppliers(string currentUser,string role,UpdateProductDTO update);
        Task<bool> DeleteProductOfSuppliers(string currentUser,string role,int productId);
    }
}
