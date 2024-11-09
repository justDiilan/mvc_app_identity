using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using mvc_app.DBContext;

namespace mvc_app.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Email and password required");
            }
            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            var result = _userManager.CreateAsync(user, password).Result;
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Customers");
            }
            return BadRequest("User is not created");
        }
        [HttpGet]
        public IActionResult Auth()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Auth(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Email and password required");
            }
            var result = _signInManager.PasswordSignInAsync(email, password, false, false).Result;
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Customers");
            }
            if (result.IsLockedOut)
            {
                return BadRequest("User is locked out");
            }
            if (result.IsNotAllowed)
            {
                return BadRequest("User is not allowed");
            }
            if (result.RequiresTwoFactor)
            {
                return BadRequest("Two factor authentication required");
            }
            return BadRequest("Invalid login attempt");
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync().Wait();
            return RedirectToAction("Index", "Customers");
        }
    }
}
