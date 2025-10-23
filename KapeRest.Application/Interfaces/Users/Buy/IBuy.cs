using KapeRest.Application.DTOs.Users.Buy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Users.Buy
{
    public interface IBuy
    {
        Task<(bool Success, string Message, object? Data)> BuyMenuItemAsync(int menuItemId);
    }
}
