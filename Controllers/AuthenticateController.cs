using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FakeRomanHsieh.API.Dtos;
using FakeRomanHsieh.API.Models;
using FakeRomanHsieh.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FakeRomanHsieh.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticateController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private ITouristRouteRepository _touristRoutesRepository;

        public AuthenticateController(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITouristRouteRepository touristRoutesRepository)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _touristRoutesRepository = touristRoutesRepository;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginDto loginDto)
        {
            // 驗證用戶名及密碼
            var loginResult = await _signInManager.PasswordSignInAsync(
                    loginDto.Email,
                    loginDto.Password,
                    false,
                    false
                );

            if (!loginResult.Succeeded)
            {
                return BadRequest("登入失敗");
            }

            var user = await _userManager.FindByNameAsync(loginDto.Email);

            // 創建JWT
            // 1.header
            String signingAlgorithm = SecurityAlgorithms.HmacSha256;
            // 2.payload
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                //new Claim(ClaimTypes.Role, "Admin")
            };
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach(var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);
            }

            // 3.signiture
            byte[] secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(secretByte);
            SigningCredentials signingCredentials = new SigningCredentials(signingKey, signingAlgorithm);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials
            );
            String tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            // return 200+ jwt
            return Ok(tokenStr);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {

            // 1.使用用戶名創建用戶對象
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };
            // 2.hash密碼 保存用戶
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest("新增使用者失敗");
            }

            // 3.初始化新用戶的購物車
            var shoppingCart = new ShoppingCart()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };
            await _touristRoutesRepository.CreateShoppingCartAsync(shoppingCart);
            await _touristRoutesRepository.SaveAsync();

            // 4.return
            return Ok();

        }
    }
}
