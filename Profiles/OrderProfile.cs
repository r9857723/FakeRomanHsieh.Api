using System;
using AutoMapper;
using FakeRomanHsieh.API.Helper;
using FakeRomanHsieh.API.Models;

namespace FakeRomanHsieh.API.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(
                    dest => dest.State,
                    opt => opt.MapFrom(src => src.State.ToString())
                );
        }
    }
}
