using System;
using System.ComponentModel.DataAnnotations;

namespace FakeRomanHsieh.API.Dtos
{
    public class RegisterDto
    {
        [Required]
        public String Email { get; set; }
        [Required]
        public String Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "密碼輸入不一致")]
        public String ConfirmPassword { get; set; }
    }
}
