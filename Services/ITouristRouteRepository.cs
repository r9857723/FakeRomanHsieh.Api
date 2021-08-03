using System;
using FakeRomanHsieh.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeRomanHsieh.API.Helper;

namespace FakeRomanHsieh.API.Services
{
    public interface ITouristRouteRepository
    {
        Task<PaginationList<TouristRoute>> GetTouristRoutesAsync(String keyWord, String operatorType, int? ratingValue, int pageNumber, int pageSize, String orderBy);
        Task<TouristRoute> GetTouristRouteAsync(Guid touristRouteId);
        Task<bool> TouristRouteExistsAsync(Guid touristRouteId);
        Task<IEnumerable<TouristRoutePicture>> GetPicturesByTouristRouteIdAsync(Guid touristRouteId);
        Task<TouristRoutePicture> GetTouristRoutePictureAsync(int TouristRoutePictureId);
        Task<IEnumerable<TouristRoute>> GetTouristRouteByIdsAsync(IEnumerable<Guid> ids);
        void AddTouristRoute(TouristRoute touristRoute);
        void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture);
        void DeleteTouristRoute(TouristRoute touristRoute);
        void DeleteTouristRoutes(IEnumerable<TouristRoute> touristRoutes);
        void DeleteTouristRoutePicture(TouristRoutePicture touristRoutePicture);
        Task<ShoppingCart> GetShoppingCartByUserIdAsync(String userId);
        Task CreateShoppingCartAsync(ShoppingCart shoppingCart);
        Task CreateShoppingCartItemAsync(LineItem lineItem);
        Task<LineItem> GetSoppingCartItemByItemIdAsync(int itemId);
        void DeleteSoppingCartItem(LineItem line);
        Task<IEnumerable<LineItem>> GetShoppingCartByIdsAsync(IEnumerable<int> ids);
        void DeleteSoppingCartItems(IEnumerable<LineItem> lineItems);
        Task CreateOrderAsync(Order order);
        Task<PaginationList<Order>> GetOrderByUserId(String Userid, int pageNumber, int pageSize);
        Task<Order> GetOrderById(Guid orderId);
        Task<bool> SaveAsync();
    }
}
