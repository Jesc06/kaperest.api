using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Interfaces.Cashiers.Buy;
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
        public BuyService(IBuy buy)
        {
            _buy = buy;
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
            return await _buy.RequestVoidAsync(saleId, reason);
        }
        public async Task<string> ApproveVoidAsync(int saleId)
        {
            return await _buy.ApproveVoidAsync(saleId);
        }
        public async Task<string> RejectVoidAsync(int saleId, string userId, string role)
        {
            return await _buy.RejectVoidAsync(saleId, userId, role);
        }

        public async Task<bool> CompleteGCashPurchaseAsync(string paymentReference, string cashierId)
        {
            return await _buy.CompleteGCashPurchaseAsync(paymentReference, cashierId);
        }

        public async Task<bool> IsGCashTransactionCompletedAsync(string paymentReference)
        {
            return await _buy.IsGCashTransactionCompletedAsync(paymentReference);
        }


    }
}
