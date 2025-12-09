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
        Task SavePendingPaymentAsync(PendingPaymentDTO dto);
        Task<PaymentVerificationResult> VerifyPaymentStatusAsync(string referenceId);
        Task<bool> CompletePendingPaymentAsync(string paymentReference, string cashierId);
        PendingPaymentDTO GetPendingPayment(string paymentReference);
    }
}
