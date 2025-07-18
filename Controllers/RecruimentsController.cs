using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Service.Interface;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PRN_Final_Project.Controllers
{
    public class RecruitmentsController : Controller
    {
        private readonly IRecruitmentService _recruitmentService;

        public RecruitmentsController(IRecruitmentService recruitmentService)
        {
            _recruitmentService = recruitmentService;
        }

        // GET: /Recruitment
        public IActionResult Index()
        {
            var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            return View();
        }

        // GET: /recruitments/detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userRoleClaim))
            {
                return RedirectToAction("Home", "Index");
            }

            if (userRoleClaim == "ADMIN")
            {
                return View("~/Views/Admin/ManageClass.cshtml");
            }

            else if (userRoleClaim == "EMPLOYEE")
            {
                return View("~/Views/Employee/manage-class.cshtml");
            }

            else if (userRoleClaim == "INTERN")
            {
                return View("~/Views/Intern/InternViewClass.cshtml");
            }

            return Redirect("/landingpage");
        }
    }
}
