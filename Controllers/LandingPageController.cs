using Microsoft.AspNetCore.Mvc;

namespace PRN_Final_Project.Controllers
{
    public class LandingPageController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/landing-page.cshtml");
        }
    }
}
