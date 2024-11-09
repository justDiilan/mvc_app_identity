using Microsoft.AspNetCore.Mvc;

namespace mvc_app.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
