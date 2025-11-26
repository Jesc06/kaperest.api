using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Application.Interfaces.Admin.TaxDiscount;
using KapeRest.Application.Interfaces.CurrentUserService;
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
        private ICurrentUser _currentUser;
        public TaxDiscountService(ITaxDiscount taxDiscount, ICurrentUser currentUser)
        {
            _taxDiscount = taxDiscount;
            _currentUser = currentUser;
        }
        #region --Tax--
        public async Task<Tax> AddTax(TaxDiscountDTO dto, string curUser, string Role)
        {
            return await _taxDiscount.AddTax(dto, curUser,Role);
        }
        public async Task<string> UpdateTax(UpdateTaxDiscountDTO dto)
        {
            var currentUser = _currentUser.Email;
            var userRole = _currentUser.Role;
            return await _taxDiscount.UpdateTax(dto, currentUser, userRole);
        }
        public async Task<ICollection> GetAllTax()
        {
            return await _taxDiscount.GetAllTax();
        }
        public async Task<string> DeleteTax(int id,string curUser, string Role)
        {
            return await _taxDiscount.DeleteTax(id, curUser, Role);
        }
        #endregion

        #region --Discount--
        public async Task<Discount> AddDiscount(TaxDiscountDTO dto)
        {
            var currentUser = _currentUser.Email;
            var userRole = _currentUser.Role;
            return await _taxDiscount.AddDiscount(dto, currentUser, userRole);
        }
        public async Task<string> UpdateDiscount(UpdateTaxDiscountDTO dto,string curUser, string Role)
        {
            return await _taxDiscount.UpdateDiscount(dto,curUser, Role);
        }
        public async Task<ICollection> GetAllDiscount()
        {
            return await _taxDiscount.GetAllDiscounts();
        }
        public async Task<string> DeleteDiscount(int id)
        {
            var currentUser = _currentUser.Email;
            var userRole = _currentUser.Role;
            return await _taxDiscount.DeleteDiscount(id, currentUser, userRole);
        }
        #endregion

    }
}
