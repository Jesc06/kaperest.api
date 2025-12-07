using KapeRest.Application.DTOs.PayMongo;
using KapeRest.Application.Interfaces.Cashiers.Buy;
using KapeRest.Application.Interfaces.PayMongo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Cashiers.PayMongo
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayGcashController : ControllerBase
    {
        private readonly IPayMongo _pay;
        private readonly IBuy _buy;
        public PayGcashController(IPayMongo pay, IBuy buy)
        {
            _pay = pay;
            _buy = buy;
        }

        [HttpPost("PayGcash")]
        public async Task<ActionResult> PayWithGcash([FromBody]CreatePaymentDTO dto)
        {
            var result = await _pay.CreateGcashPaymentAsync(dto);   
            return Ok(result);  
        }

        [HttpPost("PayMongoWebHooks")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] dynamic payload)
        {
            try
            {
                // Extract the payment reference and status from PayMongo payload
                string paymentReference = payload?.data?.id;
                string status = payload?.data?.attributes?.status;

                if (string.IsNullOrEmpty(paymentReference) || string.IsNullOrEmpty(status))
                    return BadRequest("Invalid webhook payload.");

                Console.WriteLine($"PayMongo Webhook received: Reference={paymentReference}, Status={status}");

                // If status is "chargeable", automatically complete the purchase
                if (status.ToLower() == "chargeable")
                {
                    Console.WriteLine($"Payment authorized for {paymentReference} - attempting to complete purchase...");
                    
                    // Complete the purchase automatically
                    var completed = await _buy.CompleteGCashPurchaseAsync(paymentReference, null);
                    
                    if (completed)
                    {
                        Console.WriteLine($"Purchase completed successfully for {paymentReference}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to complete purchase for {paymentReference}");
                    }
                }

                // Also update payment status as before
                await _buy.UpdatePaymentStatusAsync(paymentReference, status);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook error: {ex.Message}");
                // Log exception if needed
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("GcashQrCode")]
        public ActionResult GenerateGcashQrCode([FromQuery] string checkoutUrl)
        {
            var qrCodeBytes = _pay.GenerateQrCode(checkoutUrl);
            return File(qrCodeBytes, "image/png");
        }

        [HttpPost("SavePendingPayment")]
        public async Task<IActionResult> SavePendingPayment([FromBody] PendingPaymentDTO dto)
        {
            try
            {
                await _pay.SavePendingPaymentAsync(dto);
                return Ok(new { message = "Pending payment saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("VerifyPayment")]
        public async Task<IActionResult> VerifyPayment([FromQuery] string referenceId)
        {
            try
            {
                var result = await _pay.VerifyPaymentStatusAsync(referenceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("CheckTransactionStatus")]
        public async Task<IActionResult> CheckTransactionStatus([FromQuery] string referenceId)
        {
            try
            {
                // First check if already completed
                var alreadyCompleted = await _buy.IsGCashTransactionCompletedAsync(referenceId);
                
                if (alreadyCompleted)
                {
                    return Ok(new { 
                        completed = true,
                        message = "Transaction completed successfully"
                    });
                }

                // Not completed yet - verify payment status with PayMongo and complete if chargeable
                var verifyResult = await _pay.VerifyPaymentStatusAsync(referenceId);
                
                // If payment is authorized (chargeable), complete the purchase
                if (verifyResult != null && 
                    (verifyResult.Status?.ToLower() == "chargeable" || verifyResult.Status?.ToLower() == "authorized"))
                {
                    Console.WriteLine($"Payment authorized for {referenceId}, completing purchase...");
                    var completed = await _buy.CompleteGCashPurchaseAsync(referenceId, null);
                    
                    return Ok(new { 
                        completed = completed,
                        message = completed ? "Transaction completed successfully" : "Failed to complete transaction"
                    });
                }
                
                return Ok(new { 
                    completed = false,
                    message = $"Transaction pending. Status: {verifyResult?.Status}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
