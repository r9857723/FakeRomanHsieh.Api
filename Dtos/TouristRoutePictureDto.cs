using System;
namespace FakeRomanHsieh.API.Dtos
{
    public class TouristRoutePictureDto
    {
        public int Id { get; set; }
        public String Url { get; set; }
        public Guid TouristRouteId { get; set; }
    }
}

