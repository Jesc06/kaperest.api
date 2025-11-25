using KapeRest.Core.Entities.SalesTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Cashiers.Sales
{
    public interface IsalesTransaction
    {
        Task<ICollection<SalesTransactionEntities>> Purchases(string cashierId);
    }
}
