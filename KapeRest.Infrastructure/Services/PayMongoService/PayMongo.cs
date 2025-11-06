using KapeRest.Application.DTOs.PayMongo;
using KapeRest.Application.Interfaces.PayMongo;
using Newtonsoft.Json;
using QRCoder;
using RestSharp;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Services.PayMongoService
{
    public class PayMongo : IPayMongo
    {
        private readonly string _secretKey;
        private readonly string _baseUrl;
        public PayMongo(string secretKey)
        {
            _secretKey = secretKey;
            _baseUrl = "https://api.paymongo.com/v1";
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
                            success = "https://search.yahoo.com/search?fr=mcafee&type=E210US91215G0&p=success",
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

    }
}
