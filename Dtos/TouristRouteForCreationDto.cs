using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FakeRomanHsieh.API.ValidationAttributes;

namespace FakeRomanHsieh.API.Dtos
{
    //[TouristRouteTItleMustBeDifferentDescription]
    public class TouristRouteForCreationDto: TouristRouteForManipulationDto //: IValidatableObject
    {
        //[Required(ErrorMessage = "Title 不可空白")]
        //[MaxLength(100)]
        //public String Title { get; set; }
        //[Required]
        //[MaxLength(1500)]
        //public String Description { get; set; }
        ///// <summary>
        ///// 計算方式： 原價 ＊ 折扣
        ///// </summary>
        //public decimal Price { get; set; }
        //public DateTime CreateTime { get; set; }
        //public DateTime? UpdateTime { get; set; }
        //public DateTime? DepartureTime { get; set; }
        //public String Feature { get; set; }
        //public String Fees { get; set; }
        //public String Notes { get; set; }
        //public Double? Rating { get; set; }
        //public String TravelDays { get; set; }
        //public String TripType { get; set; }
        //public String DepartureCity { get; set; }
        //public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; } = new List<TouristRoutePictureForCreationDto>();

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return new ValidationResult(
        //            "路線名稱不可與路線說明相同",
        //            new[] { "TouristRouteForCreationDto" }
        //            );
        //    }
        //}
    }
}
