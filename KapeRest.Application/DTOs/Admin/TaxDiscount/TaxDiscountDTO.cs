using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Admin.TaxDiscount
{
    public class TaxDiscountDTO
    {
        public string SettingName { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
    }
}
