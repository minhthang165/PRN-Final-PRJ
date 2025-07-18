using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Repositories.Interface;

namespace PRN_Final_Project.Controllers
{
    public class ClassController : Controller
    {
        private readonly IClassRepository _classRepository;

        public ClassController(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (userIdClaim == "EMPLOYEE")
                return View("~/Views/Employee/manage-class.cshtml");
            else if (userIdClaim == "INTERN")
                return View("~/Views/Intern/InternViewClass.cshtml");
            else if (userIdClaim == "ADMIN")
                return View("~/Views/Admin/ManageClass.cshtml");
            else return Redirect("/landingpage");
        }
    }
}
