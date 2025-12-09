using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Core.Entities.SalesTransaction;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Cashiers.Sales
{
    public class SalesTransactionRepositories : IsalesTransaction
    {
        private readonly ApplicationDbContext _context;
        public SalesTransactionRepositories(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<SalesTransactionEntities>> Purchases(string cashierId)
        {
            var sales = await _context.SalesTransaction
                .Include(s => s.SalesItems)  // Include SalesItems to get Size and SugarLevel
                .Where(s => s.CashierId == cashierId)   // Filter based on user/cashier
                .OrderByDescending(s => s.DateTime)
                .ToListAsync();

            return sales;
        }


        public async Task<decimal> GetTotalSalesAsync()
        {
            return await _context.SalesTransaction.SumAsync(s => s.Total);
        }




    }
}
