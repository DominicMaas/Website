using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
