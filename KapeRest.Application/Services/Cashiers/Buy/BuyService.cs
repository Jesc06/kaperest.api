using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Interfaces.Cashiers.Buy;
using KapeRest.Application.Interfaces.CurrentUserService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Cashiers.Buy
{
    public class BuyService
    {
        private readonly IBuy _buy;
        private readonly ICurrentUser _currentUser;
        public BuyService(IBuy buy, ICurrentUser currentUser)
        {
            _buy = buy;
            _currentUser = currentUser;
        }
        public async Task<string> BuyItem(BuyMenuItemDTO buy)
        {
            return await _buy.BuyMenuItemAsync(buy);
        }

        public async Task<string> HoldTransaction(BuyMenuItemDTO buy)
        {
            return await _buy.HoldTransaction(buy);
        }
        public async Task<string> ResumeHoldAsync(int saleId)
        {
            return await _buy.ResumeHoldAsync(saleId);
        }
        public async Task<string> VoidItemAsync(int saleItemId, string userId, string role)
        {
            return await _buy.VoidItemAsync(saleItemId, userId, role);
        }
        public async Task<string> CancelHoldAsync(int saleId)
        {
            return await _buy.CancelHoldAsync(saleId);
        }

        public async Task<ICollection> GetHoldTransactions(string cashierId)
        {
            return await _buy.GetHoldTransactions(cashierId);
        }


        //void request from cashier
        public async Task<string> RequestVoidAsync(int saleId, string reason)
        {
            var user = _currentUser.Email;
            var role = _currentUser.Role;
            return await _buy.RequestVoidAsync(saleId, reason,user,role);
        }
        public async Task<string> ApproveVoidAsync(int saleId)
        {
            var user = _currentUser.Email;
            var role = _currentUser.Role;
            return await _buy.ApproveVoidAsync(saleId,user,role);
        }
        public async Task<string> RejectVoidAsync(int saleId)
        {
            var user = _currentUser.Email;
            var role = _currentUser.Role;
            return await _buy.RejectVoidAsync(saleId, user, role);
        }


    }
}
