using System;
using AutoMapper;
using FakeRomanHsieh.API.Dtos;
using FakeRomanHsieh.API.Models;

namespace FakeRomanHsieh.API.Profiles
{
    public class TouristRoutePictureProfile: Profile
    {
        public TouristRoutePictureProfile()
        {
            CreateMap<TouristRoutePicture, TouristRoutePictureDto>();
            CreateMap<TouristRoutePictureForCreationDto, TouristRoutePicture>();
            CreateMap<TouristRoutePicture, TouristRoutePictureForCreationDto>();
        }
    }
}
