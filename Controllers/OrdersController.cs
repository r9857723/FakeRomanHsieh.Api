using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FakeRomanHsieh.API.Helper;
using FakeRomanHsieh.API.Models;
using FakeRomanHsieh.API.ResourceParameters;
using FakeRomanHsieh.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FakeRomanHsieh.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ITouristRouteRepository _touristRoutesRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrdersController(IHttpContextAccessor httpContextAccessor, ITouristRouteRepository touristRouteRepository, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRoutesRepository = touristRouteRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 取得當前用戶訂單歷史紀錄
        /// </summary>
        [HttpGet(Name = "GetOrders")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrders([FromQuery] PaginationResourceParameters parameters)
        {
            // 獲取當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 使用userid獲取訂單歷史紀錄
            var orders = await _touristRoutesRepository.GetOrderByUserId(userId, parameters.PageNumber, parameters.PageSize);
            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        /// <summary>
        /// mmary>
        /// 取得當前用戶訂單
        /// </summary>
        [HttpGet("{orderId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrder([FromRoute] Guid orderId)
        {
            // 獲取當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var order = await _touristRoutesRepository.GetOrderById(orderId);

            return Ok(_mapper.Map<OrderDto>(order));
        }

        [HttpPost("{orderId}/placeOrder")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult>PlaceOrder([FromRoute] Guid orderId)
        {
            // 1.獲得當前用戶
            // 獲取當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2.開始處理支付
            // 取得訂單
            var order = await _touristRoutesRepository.GetOrderById(orderId);
            order.PaymentPrcoessing();
            await _touristRoutesRepository.SaveAsync();

            // 3.向第三方請求支付，等待回應
            var httpClient = _httpClientFactory.CreateClient();
            String url = @"https://localhost:5001/api/FakeVenderPaymentProcess?ordernumber={0}&returnFault={1}";

            var response = await httpClient.PostAsync(
                String.Format(url, order.Id, false),
                null
                );

            // 4.提取支付結果及訊息
            var isApproved = false;
            String transcationMetadata = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                transcationMetadata = await response.Content.ReadAsStringAsync();
                var jsonObject = (JObject)JsonConvert.DeserializeObject(transcationMetadata);
                isApproved = jsonObject["approved"].Value<bool>();
            }

            // 5.支付成功則完成訂單
            if (isApproved)
            {
                order.PaymenyApprove();
            } else
            {
                order.PaymentReject();
            }
            order.TranscationMetadata = transcationMetadata;
            await _touristRoutesRepository.SaveAsync();
            return Ok(_mapper.Map<OrderDto>(order));
        }
    }
}
