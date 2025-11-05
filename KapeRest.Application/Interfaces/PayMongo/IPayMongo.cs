using KapeRest.Application.DTOs.PayMongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.PayMongo
{
    public interface IPayMongo
    {
        Task<PaymentResultDto> CreateGcashPaymentAsync(CreatePaymentDTO dto);
        byte[] GenerateQrCode(string checkoutUrl);
    }
}
