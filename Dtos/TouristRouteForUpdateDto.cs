using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FakeRomanHsieh.API.Dtos
{
    public class TouristRouteForUpdateDto: TouristRouteForManipulationDto
    {
        [Required(ErrorMessage = "更新必備")]
        [MaxLength(1500)]
        public override String Description { get; set; }

    }
}
