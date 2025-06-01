using KapeRest.Application.DTOs.PayMongo;
using KapeRest.Application.Interfaces.PayMongo;
using KapeRest.Core.Entities.PendingPaymentEntities;
using KapeRest.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QRCoder;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Infrastructure.Persistence.Database;

namespace KapeRest.Infrastructure.Services.PayMongoService
{
    public class PayMongo : IPayMongo
    {
        private readonly string _secretKey;
        private readonly string _baseUrl;
        private readonly ApplicationDbContext _context;
        
        public PayMongo(string secretKey, ApplicationDbContext context)
        {
            _secretKey = secretKey;
            _baseUrl = "https://api.paymongo.com/v1";
            _context = context;
        }

        public async Task<PaymentResultDto> CreateGcashPaymentAsync(CreatePaymentDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Amount <= 0) throw new ArgumentException("Amount must be greater than zero.");

            var client = new RestClient($"{_baseUrl}/sources");
            var request = new RestRequest();
            request.Method = Method.Post;

            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(_secretKey + ":"));
            request.AddHeader("Authorization", $"Basic {authToken}");
            request.AddHeader("Content-Type", "application/json");

            var payload = new
            {
                data = new
                {
                    attributes = new
                    {
                        amount = (int)(dto.Amount * 100), // in centavos
                        currency = "PHP",
                        type = "gcash",
                        redirect = new
                        {
                            success = "https://www.youtube.com/results?search_query=pay",
                            failed = "https://example.com/failed"
                        }
                    }
                }
            };

            request.AddJsonBody(payload);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception($"PayMongo error ({response.StatusCode}): {response.Content}");

            dynamic result = JsonConvert.DeserializeObject(response.Content!)!;
            string checkoutUrl = result?.data?.attributes?.redirect?.checkout_url!;

            if (string.IsNullOrEmpty(checkoutUrl))
                throw new Exception("Checkout URL not found in PayMongo response.");

            return new PaymentResultDto
            {
                CheckoutUrl = checkoutUrl,
                ReferenceId = result.data.id
            };
        }

        public byte[] GenerateQrCode(string checkoutUrl)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(checkoutUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrData);
            using var bitmap = qrCode.GetGraphic(20);
            using var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }

        // Database storage for pending payments
        public async Task SavePendingPaymentAsync(PendingPaymentDTO dto)
        {
            if (string.IsNullOrEmpty(dto.PaymentReference))
                throw new ArgumentException("Payment reference is required");

            Console.WriteLine($"💾 Saving pending payment to DATABASE:");
            Console.WriteLine($"   Reference: {dto.PaymentReference}");
            Console.WriteLine($"   CashierId: {dto.CashierId}");
            Console.WriteLine($"   BranchId: {dto.BranchId}");
            Console.WriteLine($"   Items: {dto.CartItems.Count}");
            Console.WriteLine($"   Total: ₱{dto.TotalAmount:F2}");

            // Check if already exists
            var existing = await _context.PendingGCashPayments
                .FirstOrDefaultAsync(p => p.PaymentReference == dto.PaymentReference);

            if (existing != null)
            {
                // Update existing
                existing.CashierId = dto.CashierId;
                existing.BranchId = dto.BranchId;
                existing.DiscountPercent = dto.DiscountPercent;
                existing.TaxPercent = dto.TaxPercent;
                existing.TotalAmount = dto.TotalAmount;
                existing.CartItemsJson = JsonConvert.SerializeObject(dto.CartItems);
                existing.CreatedAt = DateTime.Now;
                
                _context.PendingGCashPayments.Update(existing);
            }
            else
            {
                // Create new
                var entity = new PendingGCashPaymentEntities
                {
                    PaymentReference = dto.PaymentReference,
                    CashierId = dto.CashierId,
                    BranchId = dto.BranchId,
                    DiscountPercent = dto.DiscountPercent,
                    TaxPercent = dto.TaxPercent,
                    TotalAmount = dto.TotalAmount,
                    CartItemsJson = JsonConvert.SerializeObject(dto.CartItems),
                    CreatedAt = DateTime.Now,
                    IsCompleted = false
                };

                await _context.PendingGCashPayments.AddAsync(entity);
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"Pending payment saved to database successfully!");
        }

        public async Task<PaymentVerificationResult> VerifyPaymentStatusAsync(string referenceId)
        {
            if (string.IsNullOrEmpty(referenceId))
                throw new ArgumentException("Reference ID is required");

            try
            {
                // Call PayMongo API to check payment source status
                var client = new RestClient($"{_baseUrl}/sources/{referenceId}");
                var request = new RestRequest();
                request.Method = Method.Get;

                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(_secretKey + ":"));
                request.AddHeader("Authorization", $"Basic {authToken}");
                request.AddHeader("Content-Type", "application/json");

                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    return new PaymentVerificationResult
                    {
                        Status = "failed",
                        PaymentReference = referenceId,
                        IsCompleted = false,
                        Message = $"Failed to verify payment: {response.Content}"
                    };
                }

                dynamic result = JsonConvert.DeserializeObject(response.Content!)!;
                string status = result?.data?.attributes?.status?.ToString() ?? "pending";

                // PayMongo source statuses: chargeable, pending, cancelled, expired
                // "chargeable" means customer has authorized the payment
                bool isCompleted = status.ToLower() == "chargeable";

                return new PaymentVerificationResult
                {
                    Status = status,
                    PaymentReference = referenceId,
                    IsCompleted = isCompleted,
                    Message = isCompleted ? "Payment authorized successfully" : $"Payment status: {status}"
                };
            }
            catch (Exception ex)
            {
                return new PaymentVerificationResult
                {
                    Status = "error",
                    PaymentReference = referenceId,
                    IsCompleted = false,
                    Message = $"Error verifying payment: {ex.Message}"
                };
            }
        }

        public async Task<bool> CompletePendingPaymentAsync(string paymentReference, string cashierId)
        {
            if (string.IsNullOrEmpty(paymentReference))
                throw new ArgumentException("Payment reference is required");

            // Check if exists in database and not completed
            var exists = await _context.PendingGCashPayments
                .AnyAsync(p => p.PaymentReference == paymentReference && !p.IsCompleted);
            
            if (!exists)
            {
                Console.WriteLine($"❌ No pending payment found in database for {paymentReference}");
                return false;
            }

            Console.WriteLine($"✅ Pending payment found for {paymentReference} - ready for completion");
            return true;
        }

        public PendingPaymentDTO GetPendingPayment(string paymentReference)
        {
            var entity = _context.PendingGCashPayments
                .FirstOrDefault(p => p.PaymentReference == paymentReference && !p.IsCompleted);

            if (entity == null)
            {
                Console.WriteLine($"❌ No pending payment found in database for {paymentReference}");
                return null;
            }

            Console.WriteLine($"✅ Found pending payment in database for {paymentReference}");
            Console.WriteLine($"   CashierId: {entity.CashierId}");
            Console.WriteLine($"   BranchId: {entity.BranchId}");

            // Deserialize cart items
            var cartItems = JsonConvert.DeserializeObject<List<CartItemDTO>>(entity.CartItemsJson) ?? new List<CartItemDTO>();

            var dto = new PendingPaymentDTO
            {
                PaymentReference = entity.PaymentReference,
                CashierId = entity.CashierId,
                BranchId = entity.BranchId,
                DiscountPercent = entity.DiscountPercent,
                TaxPercent = entity.TaxPercent,
                TotalAmount = entity.TotalAmount,
                CartItems = cartItems
            };

            // Mark as completed
            entity.IsCompleted = true;
            _context.PendingGCashPayments.Update(entity);
            _context.SaveChanges();
            Console.WriteLine($"✅ Marked payment as completed in database");

            return dto;
        }

    }
}
