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
        Task<ProductResponseDTO> AddProduct(string currentUser,string role,CreateProductDTO addProduct);
        Task<ProductResponseDTO> UpdateProduct(string currentUser,string role,UpdateProductDTO update);
        Task<bool> DeleteProduct(string currentUser,string role,int productId);
    }
}
