using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Application.Interfaces.Admin.TaxDiscount;
using KapeRest.Core.Entities.Tax_Rate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.TaxDiscount
{
    public class TaxDiscountService
    {
        private readonly ITaxDiscount _taxDiscount;
        public TaxDiscountService(ITaxDiscount taxDiscount)
        {
            _taxDiscount = taxDiscount;
        }
        #region --Tax--
        public async Task<Tax> AddTax(TaxDiscountDTO dto)
        {
            return await _taxDiscount.AddTax(dto);
        }
        public async Task<string> UpdateTax(UpdateTaxDiscountDTO dto)
        {
            return await _taxDiscount.UpdateTax(dto);
        }
        public async Task<ICollection> GetAllTax()
        {
            return await _taxDiscount.GetAllTax();
        }
        public async Task<string> DeleteTax(int id)
        {
            return await _taxDiscount.DeleteTax(id);
        }
        #endregion

        #region --Discount--
        public async Task<Discount> AddDiscount(TaxDiscountDTO dto)
        {
            return await _taxDiscount.AddDiscount(dto);
        }
        public async Task<string> UpdateDiscount(UpdateTaxDiscountDTO dto)
        {
            return await _taxDiscount.UpdateDiscount(dto);
        }
        public async Task<ICollection> GetAllDiscount()
        {
            return await _taxDiscount.GetAllDiscounts();
        }
        public async Task<string> DeleteDiscount(int id)
        {
            return await _taxDiscount.DeleteDiscount(id);
        }
        #endregion

    }
}
