using KapeRest.Application.Interfaces.Users.Sales;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Users.Sales
{
    public class OverAllSalesService
    {
        private readonly IOverallSales _overallSales;
        public OverAllSalesService(IOverallSales overallSales)
        {
            _overallSales = overallSales;
        }

        public async Task<ICollection> GetAllSalesByCashiers(string cashierId)
        {
            return await _overallSales.GetAllSalesByCashiers(cashierId);
        }
        public async Task<ICollection> GetAllSalesByAdmin()
        {
            return await _overallSales.GetAllSalesByAdmin();
        }

    }
}
