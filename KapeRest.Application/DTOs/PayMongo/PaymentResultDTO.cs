using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.PayMongo
{
    public class PaymentResultDto
    {
        public string CheckoutUrl { get; set; }
        public string ReferenceId { get; set; }
    }
}
