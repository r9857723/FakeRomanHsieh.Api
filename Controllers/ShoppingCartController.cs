using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FakeRomanHsieh.API.Dtos;
using FakeRomanHsieh.API.Helper;
using FakeRomanHsieh.API.Models;
using FakeRomanHsieh.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeRomanHsieh.API.Controllers
{
    [ApiController]
    [Route("api/ShoppingCart")]
    public class ShoppingCartController: ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ITouristRouteRepository _touristRoutesRepository;
        private readonly IMapper _mapper;

        public ShoppingCartController(IHttpContextAccessor httpContextAccessor, ITouristRouteRepository touristRouteRepository, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRoutesRepository = touristRouteRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// 取得用戶購物車內容
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetShoppingCart")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetShoppingCart()
        {
            // 獲取當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 使用userId獲得用戶的購物車
            var shoppingCart = await _touristRoutesRepository.GetShoppingCartByUserIdAsync(userId);

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart)); ;

        }

        /// <summary>
        /// 新增旅遊路線至購物車
        /// </summary>
        /// <param name="shoppingCartForCreationDto"></param>
        /// <returns></returns>
        [HttpPost("items")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CreateShoppingCartItem([FromBody] ShoppingCartForCreationDto shoppingCartForCreationDto)
        {
            // 1.獲取當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 2.使用userId獲得用戶的購物車
            var shoppingCart = await _touristRoutesRepository.GetShoppingCartByUserIdAsync(userId);

            // 3.創建lineItem 
            TouristRoute touristRoute = await _touristRoutesRepository.GetTouristRouteAsync(shoppingCartForCreationDto.TouristRouteId);
            if (touristRoute == null)
            {
                return BadRequest("找不到旅遊路線");
            }

            var lineItem = new LineItem()
            {
                TouristRouteId = shoppingCartForCreationDto.TouristRouteId,
                ShoppingCartId = shoppingCart.Id,
                OriginalPrice = touristRoute.OriginalPrice,
                DiscountPresent = touristRoute.DiscountPresent
            };

            // 4.添加進數據庫

            await _touristRoutesRepository.CreateShoppingCartItemAsync(lineItem);
            await _touristRoutesRepository.SaveAsync();

            return Ok(_mapper.Map<ShoppingCartDto>(shoppingCart));
        }

        /// <summary>
        /// 刪除購物車中單一旅遊路徑
        /// </summary>
        /// <returns></returns>
        [HttpDelete("items/{itemId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItem([FromRoute] int itemId)
        {
            var lineItem =  await _touristRoutesRepository.GetSoppingCartItemByItemIdAsync(itemId);
            if (lineItem == null) 
            {
                return NotFound("購物車商品找不到");
            }

            _touristRoutesRepository.DeleteSoppingCartItem(lineItem);
            await _touristRoutesRepository.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 刪除購物車中多筆旅遊路徑
        /// </summary>
        /// <returns></returns>
        [HttpDelete("items/({itemIds})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> DeleteShoppingCartItmes(
            [ModelBinder(BinderType =typeof(ArrayModelBinder))]
            [FromRoute] IEnumerable<int> itemIds)
        {
            IEnumerable<LineItem> lineItems = await _touristRoutesRepository.GetShoppingCartByIdsAsync(itemIds);

            _touristRoutesRepository.DeleteSoppingCartItems(lineItems);
            await _touristRoutesRepository.SaveAsync();
            return NoContent();
        }

        /// <summary>
        /// 訂單結算
        /// </summary>
        /// <returns></returns>
        [HttpPost("checkout")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> CheckOut()
        {
            // 獲取當前用戶
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // 使用userId獲得用戶的購物車
            var shoppingCart = await _touristRoutesRepository.GetShoppingCartByUserIdAsync(userId);
            // 創建訂單
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                State = OrderStateEnum.Pending,
                OrderItems = shoppingCart.ShoppingCartItems,
                CreateDateUtc = DateTime.UtcNow,
            };
            // 清空購物車
            shoppingCart.ShoppingCartItems = null;
            // 保存訂單
            await _touristRoutesRepository.CreateOrderAsync(order);
            await _touristRoutesRepository.SaveAsync();
            return Ok(_mapper.Map<OrderDto>(order));
        }
    }
}
