using System;
using System.Collections.Generic;
using FakeRomanHsieh.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FakeRomanHsieh.API.Controllers
{
    [Route("api", Name = "GetRoot")]
    [ApiController]
    public class RouteController: ControllerBase
    {
        [HttpGet]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            // 自我鏈接
            links.Add(
                new LinkDto(
                    Url.Link("GetRoot", null),
                    "self",
                    "GET"
                ));

            // 一級鏈接 旅遊路線 "GET  api/touristRoutes"
            links.Add(
                new LinkDto(
                    Url.Link("GetTouistRoutes", null),
                    "get_tourist_route",
                    "GET"
                ));

            // 一級鏈接 旅遊路線 "POST  api/touristRoutes"
            links.Add(
                new LinkDto(
                    Url.Link("CreateTouristRoute", null),
                    "create_tourist_route",
                    "POST"
                ));

            // 一級鏈接 購物車 "GET api/orders"
            links.Add(
                new LinkDto(
                    Url.Link("GetShoppingCart", null),
                    "get_shopping_cart",
                    "GET"
                ));

            // 一級鏈接 訂單 "GET api/shoppingCart"
            links.Add(
                new LinkDto(
                    Url.Link("GetOrders", null),
                    "get_orders",
                    "GET"
                ));
            return Ok(links);
        }
    }
}
