using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Cashiers.Sales
{
    public interface IOverallSales
    {
        Task<ICollection> GetAllSalesByCashiers(string cashierId);
        Task<ICollection> GetAllSalesByAdmin();
    }
}
