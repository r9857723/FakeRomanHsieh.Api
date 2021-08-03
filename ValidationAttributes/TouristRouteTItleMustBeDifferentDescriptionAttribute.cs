using System.ComponentModel.DataAnnotations;
using FakeRomanHsieh.API.Dtos;

namespace FakeRomanHsieh.API.ValidationAttributes
{
    public class TouristRouteTItleMustBeDifferentDescriptionAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            TouristRouteForManipulationDto touristRouteForManipulationDto = (TouristRouteForManipulationDto)validationContext.ObjectInstance;

            if (touristRouteForManipulationDto.Title == touristRouteForManipulationDto.Description)
            {
                return new ValidationResult(
                    "路線名稱不可與路線說明相同",
                    new[] { "TouristRouteForCreationDto" }
                    );
            };
            return ValidationResult.Success;
        }
    }
}
