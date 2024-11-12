using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace mvc_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIUserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public APIUserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return BadRequest("Email and password required");
            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded) return RedirectToAction("Index", "Customers");
            return BadRequest("Invalid registration attempt");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return BadRequest("Email and password required");
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded) return RedirectToAction("Index", "Customers");
            if (result.IsLockedOut) return BadRequest("User is locked out");
            if (result.IsNotAllowed) return BadRequest("User is not allowed");
            if (result.RequiresTwoFactor) return BadRequest("Two factor authentication required");
            return BadRequest("Invalid login attempt");
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
    }
}
