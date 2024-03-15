using Microsoft.AspNetCore.Mvc;

namespace PizzaApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
