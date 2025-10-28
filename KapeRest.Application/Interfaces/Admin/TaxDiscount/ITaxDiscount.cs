using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Core.Entities.Tax_Rate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Admin.TaxDiscount
{
    public interface ITaxDiscount
    {
        #region --Tax--
        Task<Tax> AddTax(TaxDiscountDTO dto);
        Task<string> UpdateTax(UpdateTaxDiscountDTO dto);
        Task<ICollection> GetAllTax();
        Task<string> DeleteTax(int id);
        #endregion

        #region --Discount--
        Task<Discount> AddDiscount(TaxDiscountDTO dto);
        Task<string> UpdateDiscount(UpdateTaxDiscountDTO dto);
        Task<ICollection> GetAllDiscounts();
        Task<string> DeleteDiscount(int id);
        #endregion

    }
}
