using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Application.Interfaces.Admin.TaxDiscount;
using KapeRest.Core.Entities.Tax_Rate;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Admin.TaxDiscount
{
    public class TaxDiscountRepository : ITaxDiscount
    {
        private readonly ApplicationDbContext _context;
        public TaxDiscountRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        #region --Tax--
        public async Task<Tax> AddTax(TaxDiscountDTO dto)
        {
            var rate = new Tax
            {
                TaxRate = dto.SettingName,
                Value = dto.Value,
                Description = dto.Description
            };
            _context.Tax.Add(rate);
            await _context.SaveChangesAsync();
            return rate;
        }

        public async Task<string> UpdateTax(UpdateTaxDiscountDTO dto)
        {
            var taxDiscount = await _context.Tax.FirstOrDefaultAsync(t => t.Id == dto.Id);
            if(taxDiscount is not null)
            {
                taxDiscount.TaxRate = dto.SettingName;
                taxDiscount.Value = dto.Value;
                taxDiscount.Description = dto.Description;
                await _context.SaveChangesAsync();
                return "Successfully Updated";
            }
            return "failed to update";
        }

        public async Task<ICollection> GetAllTax()
        {
            var taxAndDiscounts = await _context.Tax.ToListAsync();
            return taxAndDiscounts;
        }

        public async Task<string> DeleteTax(int id)
        {
            var taxAndDiscounts = await _context.Tax.FindAsync(id);
            if(taxAndDiscounts is null)
                return "Tax and Discount not found";

            _context.Tax.Remove(taxAndDiscounts);
            await _context.SaveChangesAsync();
            return "Successfully Deleted";
        }
        #endregion

        #region --Discount--
        public async Task<Discount> AddDiscount(TaxDiscountDTO dto)
        {
            var rate = new Discount
            {
                TaxRate = dto.SettingName,
                Value = dto.Value,
                Description = dto.Description
            };
            _context.Discount.Add(rate);
            await _context.SaveChangesAsync();
            return rate;
        }
        public async Task<string> UpdateDiscount(UpdateTaxDiscountDTO dto)
        {
            var taxDiscount = await _context.Discount.FirstOrDefaultAsync(t => t.Id == dto.Id);
            if (taxDiscount is not null)
            {
                taxDiscount.TaxRate = dto.SettingName;
                taxDiscount.Value = dto.Value;
                taxDiscount.Description = dto.Description;
                await _context.SaveChangesAsync();
                return "Successfully Updated";
            }
            return "failed to update";
        }
        public async Task<ICollection> GetAllDiscounts()
        {
            var taxAndDiscounts = await _context.Discount.ToListAsync();
            return taxAndDiscounts;
        }
        public async Task<string> DeleteDiscount(int id)
        {
            var taxAndDiscounts = await _context.Discount.FindAsync(id);
            if (taxAndDiscounts is null)
                return "Tax and Discount not found";

            _context.Discount.Remove(taxAndDiscounts);
            await _context.SaveChangesAsync();
            return "Successfully Deleted";
        }
        #endregion



    }
}
