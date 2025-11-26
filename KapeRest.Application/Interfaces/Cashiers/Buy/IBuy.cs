using KapeRest.Application.DTOs.Users.Buy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Cashiers.Buy
{
    public interface IBuy
    {
        Task<string> BuyMenuItemAsync(BuyMenuItemDTO buy);
        Task<string> HoldTransaction(BuyMenuItemDTO buy);
        Task<string> ResumeHoldAsync(int saleId);
        Task<string> CancelHoldAsync(int saleId);
        Task<string> VoidItemAsync(int saleId, string userId, string role);
        Task<ICollection> GetHoldTransactions(string cashierId);


        //Void request
        Task<string> RequestVoidAsync(int saleId, string reason, string user, string role);
        Task<string> ApproveVoidAsync(int saleId, string user, string role);
        Task<string> RejectVoidAsync(int saleId, string userId, string role);

    }
}
