using System;
using System.ComponentModel.DataAnnotations;

namespace FakeRomanHsieh.API.Dtos
{
    public class LoginDto
    {
        [Required]
        public String Email { get; set; }
        [Required]
        public String Password { get; set; }
    }
}
