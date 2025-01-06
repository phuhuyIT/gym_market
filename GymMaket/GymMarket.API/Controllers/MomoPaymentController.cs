using GymMarket.API.Models;
using GymMarket.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GymMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MomoPaymentController : ControllerBase
    {
        private MomoService _momoService;

        public MomoPaymentController(MomoService momoService)
        {
            _momoService = momoService;

        }

        [HttpPost]
        [Route("CreatePaymentUrl")]
        public async Task<IActionResult> CreatePaymentUrl()
        {
            var response = await _momoService.CreatePaymentAsync();
            return Redirect(response.PayUrl);
        }

        [HttpGet]
        [Route("PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack()
        {
            return Ok(new { message = "PaymentCallBack" });
        }

        [HttpGet]
        [Route("MomoNotify")]
        public async Task<IActionResult> MomoNotify()
        {
            return Ok(new { message = "MomoNotify" });
        }
    }
}
