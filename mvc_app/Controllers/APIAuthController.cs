using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using mvc_app.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace mvc_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIAuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public APIAuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Models.RegisterModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully" });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Models.LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid login attempt");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid login attempt");
        }

        [HttpPost]
        public async Task<IActionResult> LogoutConfirmed()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName)) return BadRequest("Role name required");
            var roleExisting = await _roleManager.RoleExistsAsync(roleName);
            if (roleExisting) BadRequest($"Role name {roleName} already exists");
            var role = new IdentityRole { Name = roleName };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded) return RedirectToAction("Index", "Home");
            return BadRequest("Invalid create role attempt");
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> AssignRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
                return BadRequest("User ID and Role Name required");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");
            var roleExisting = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExisting) BadRequest($"Role name {roleName} not exists");
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded) return RedirectToAction("Index", "Home");
            return BadRequest($"Invalid assign role {roleName} attempt for user {user}");
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            // Обновление SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                //Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                Expires = DateTime.UtcNow.AddDays(30),  // Токен истекает через 30 минут
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = _configuration["Jwt:Issuer"],   // Добавлено
                Audience = _configuration["Jwt:Audience"] // Добавлено
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
