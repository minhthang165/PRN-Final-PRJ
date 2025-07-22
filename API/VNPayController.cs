using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace PRN_Final_Project.API
{
    public class VNPayController : Controller
    {
        public static double money;
        private readonly IVnpay _vnpay;

        public VNPayController(IVnpay vnpay)
        {
            _vnpay = vnpay;
        }

        [HttpGet("Donation")]
        public IActionResult Index()
        {
            return View("~/Views/Donation.cshtml");
        }

        [HttpGet("CreatePaymentUrl")]
        public ActionResult<string> CreatePaymentUrl(double moneyToPay, string description)
        {
            money = moneyToPay;
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = moneyToPay,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                    CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                    Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                    Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return Ok(paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("IpnAction")]
        public IActionResult IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                        return Ok();
                    }

                    // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }

        [HttpGet("Callback")]
        public ActionResult<string> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";
                    ViewData["PaymentSuccess"] = true;
                    ViewData["Money"] = money;
                    ViewData["PaymentResult"] = paymentResult;
                    ViewData["ResultDescription"] = resultDescription;
                    return View("~/Views/Donation.cshtml");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }
}
