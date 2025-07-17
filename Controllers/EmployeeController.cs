using Microsoft.AspNetCore.Mvc;

namespace PRN_Final_Project.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
