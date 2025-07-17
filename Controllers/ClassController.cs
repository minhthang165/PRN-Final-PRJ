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
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var classes = _classRepository.GetClassesByMentorId(Convert.ToInt32(userIdClaim));
            return View("~/Views/Employee/manage-class.cshtml", classes);
        }
    }
}
