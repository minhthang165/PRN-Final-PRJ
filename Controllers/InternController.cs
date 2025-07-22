using Microsoft.AspNetCore.Mvc;

namespace PRN_Final_Project.Controllers
{
    public class InternController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
