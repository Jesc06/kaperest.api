using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.PayMongo
{
    public class CreatePaymentDTO
    {
        public decimal Amount { get; set; } 
        public string Description { get; set; }
    }
}
