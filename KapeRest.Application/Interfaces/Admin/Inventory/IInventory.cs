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
        Task<ProductResponseDTO> AddProduct(string currentUser,CreateProductDTO addProduct);
        Task<ProductResponseDTO> UpdateProduct(UpdateProductDTO update);
        Task<bool> DeleteProduct(int productId);
    }
}
