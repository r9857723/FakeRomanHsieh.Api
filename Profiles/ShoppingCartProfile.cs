using System;
using AutoMapper;
using FakeRomanHsieh.API.Dtos;
using FakeRomanHsieh.API.Models;

namespace FakeRomanHsieh.API.Profiles
{
    public class ShoppingCartProfile: Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<LineItem, LineItemDto>();
        }
    }
}
