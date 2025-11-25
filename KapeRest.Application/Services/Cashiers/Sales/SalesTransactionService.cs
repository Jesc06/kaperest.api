using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Core.Entities.SalesTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Cashiers.Sales
{
    public class SalesTransactionService
    {
        private readonly IsalesTransaction _transaction;
        public SalesTransactionService(IsalesTransaction transaction)
        {
           _transaction = transaction;
        }

        public async Task<ICollection<SalesTransactionEntities>>SalesPurchases(string CashierId)
        {
            var sales = await _transaction.Purchases(CashierId);
            return sales;
        }



    }
}
