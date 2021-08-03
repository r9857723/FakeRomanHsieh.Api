using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeRomanHsieh.API.Database;
using FakeRomanHsieh.API.Dtos;
using FakeRomanHsieh.API.Helper;
using FakeRomanHsieh.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FakeRomanHsieh.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        private readonly AppDbContext _context;
        private readonly IPropertyMappingService _propertyMappingService;
        public TouristRouteRepository(AppDbContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        /// <summary>
        /// 取得所有旅遊路徑
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="operatorType"></param>
        /// <param name="ratingValue"></param>
        /// <returns></returns>
        public async Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(String keyWord, String operatorType, int? ratingValue, int pageNumber, int pageSize, String orderBy)
        {
            IQueryable<TouristRoute> result = _context.TouristRoutes.Include(x => x.TouristRoutePictures);
            if (!String.IsNullOrWhiteSpace(keyWord))
            {
                keyWord = keyWord.Trim();
                result = result.Where(x => x.Title.Contains(keyWord));
            }

            if (ratingValue >= 0)
            {
                operatorType = operatorType.ToLower();
                result = operatorType switch
                {
                    "largerthan" => result.Where(x => x.Rating > ratingValue),
                    "lessthan" => result.Where(x => x.Rating < ratingValue),
                    _ => result.Where(x => x.Rating == ratingValue),
                }; 
            }

            if (!String.IsNullOrWhiteSpace(orderBy))
            {
                var touristRouteMappingDistionary = _propertyMappingService.GetPropertyMapping<TouristRouteDto, TouristRoute>();
                result = result.ApplySort(orderBy, touristRouteMappingDistionary);
            }

            return await PaginationList<TouristRoute>.CreateAsync(pageNumber, pageSize, result);
        }

        /// <summary>
        /// 取得旅遊路徑
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        public async Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.Include(x => x.TouristRoutePictures).FirstOrDefaultAsync(x => x.Id == touristRouteId);
        }

        /// <summary>
        /// 判斷旅遊路徑是否存在
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        public async Task<bool> TouristRouteExistsAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutes.AnyAsync(x => x.Id == touristRouteId);
        }

        /// <summary>
        /// 取得旅遊路徑所有圖片
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId)
        {
            return await _context.TouristRoutePictures.Where(x => x.TouristRouteId == touristRouteId).ToListAsync();
        }

        /// <summary>
        /// 取得旅遊路徑圖片
        /// </summary>
        /// <param name="TouristRoutePictureId"></param>
        /// <returns></returns>
        public async Task<TouristRoutePicture> GetTouristRoutePictureAsync(int TouristRoutePictureId)
        {
            return await _context.TouristRoutePictures.Where(x => x.Id == TouristRoutePictureId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 查詢多筆旅遊路線
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TouristRoute>> GetTouristRouteByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.TouristRoutes.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        /// <summary>
        /// 新增旅遊路徑
        /// </summary>
        /// <param name="touristRoute"></param>
        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute == null)
            {
                throw new ArgumentNullException(nameof(touristRoute));
            }
            _context.TouristRoutes.Add(touristRoute);
        }

        /// <summary>
        /// 新增旅遊路徑圖片
        /// </summary>
        /// <param name="touristRouteId"></param>
        /// <param name="touristRoutePicture"></param>
        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (touristRouteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(touristRouteId));
            }
            if (touristRoutePicture == null)
            {
                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }

        /// <summary>
        /// 刪除旅遊路線＋圖片
        /// </summary>
        /// <param name="touristRoute"></param>
        public void DeleteTouristRoute(TouristRoute touristRoute)
        {
            _context.TouristRoutes.Remove(touristRoute);
        }

        /// <summary>
        /// 刪除旅遊路線的照片
        /// </summary>
        /// <param name="touristRoutePicture"></param>
        public void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture)
        {
            _context.TouristRoutePictures.Remove(touristRoutePicture);
        }

        /// <summary>
        /// 刪除多筆旅遊路線
        /// </summary>
        /// <param name="touristRoutes"></param>
        public void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes)
        {
            _context.TouristRoutes.RemoveRange(touristRoutes);
        }

        /// <summary>
        /// 取得用戶的購物車
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ShoppingCart> GetShoppingCartByUserIdAsync(String userId) {
            return await _context.ShoppingCarts
                .Include(x => x.User)
                .Include(x => x.ShoppingCartItems)
                .ThenInclude(x => x.TouristRoute)
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 創建用戶的購物車
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        public async Task CreateShoppingCartAsync(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
        }

        /// <summary>
        /// 將lineItem添加進數據庫
        /// </summary>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        public async Task CreateShoppingCartItemAsync(LineItem lineItem)
        {
            await _context.LineItems.AddAsync(lineItem);
        }

        /// <summary>
        /// 獲取購物車商品資訊
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public async Task<LineItem> GetSoppingCartItemByItemIdAsync(int itemId)
        {
            return await _context.LineItems.FirstOrDefaultAsync(x => x.Id == itemId);
        }

        /// <summary>
        /// 刪除購物車中單一旅遊路徑
        /// </summary>
        /// <param name="lineItem"></param>
        public void DeleteSoppingCartItem(LineItem lineItem)
        {
            _context.LineItems.Remove(lineItem);
        }

        /// <summary>
        /// 取得購物車所有旅遊路線
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LineItem>> GetShoppingCartByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.LineItems.Where(x => ids.Contains(x.Id)).ToListAsync();
        }
        /// <summary>
        /// 刪除購物車中多筆旅遊路徑
        /// </summary>
        /// <param name="lineItems"></param>
        public void DeleteSoppingCartItems(IEnumerable<LineItem> lineItems)
        {
            _context.LineItems.RemoveRange(lineItems);
        }

        /// <summary>
        /// 創建訂單
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        /// <summary>
        /// 取得當前用戶訂單歷史紀錄
        /// </summary>
        /// <param name="Userid"></param>
        /// <returns></returns>
        public async Task<PaginationList<Order>> GetOrderByUserId(String Userid, int pageNumber, int pageSize)
        {
            IQueryable<Order> result = _context.Orders.Where(x => x.UserId == Userid);
            return await PaginationList<Order>.CreateAsync(pageNumber, pageSize, result);
            //return await _context.Orders.Where(x => x.UserId == userid).ToListAsync();
        }

        /// <summary>
        /// 取得當前用戶訂單
        /// </summary>
        /// <param name="Userid"></param>
        /// <returns></returns>
        public async Task<Order> GetOrderById(Guid orderId)
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.TouristRoute)
                .FirstOrDefaultAsync(x => x.Id == orderId);
        }
        /// <summary>
        /// 存檔
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
