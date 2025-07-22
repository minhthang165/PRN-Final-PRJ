using Microsoft.AspNetCore.Mvc;

namespace PRN_Final_Project.Controllers
{
    public class MessengerController : Controller
    {
        [HttpGet("/messenger")]
        public IActionResult Index()
        {
            return View("~/Views/messenger.cshtml");
        }
    }
}
