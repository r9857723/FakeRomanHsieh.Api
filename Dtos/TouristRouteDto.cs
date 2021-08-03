using System;
using System.Collections.Generic;

namespace FakeRomanHsieh.API.Dtos
{
    public class TouristRouteDto
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        /// <summary>
        /// 計算方式： 原價 ＊ 折扣
        /// </summary>
        public decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public String Feature { get; set; }
        public String Fees { get; set; }
        public String Notes { get; set; }
        public Double? Rating { get; set; }
        public String TravelDays { get; set; }
        public String TripType { get; set; }
        public String DepartureCity { get; set; }
        public ICollection<TouristRoutePictureDto> TouristRoutePictures { get; set; }
    }
}
