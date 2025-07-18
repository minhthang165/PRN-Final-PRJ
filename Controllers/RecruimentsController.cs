using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Service.Interface;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PRN_Final_Project.Controllers
{
    public class RecruitmentsController : Controller
    {
        private readonly IRecruitmentService _recruitmentService;
        private readonly IUserService _userService;

        public RecruitmentsController(IRecruitmentService recruitmentService, IUserService userService)
        {
            _recruitmentService = recruitmentService;
            _userService = userService;
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
            var recruitment = await _recruitmentService.GetByIdAsync(id);
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (recruitment == null || userIdClaim == null)
            {
                return RedirectToAction("Index", "Recruitments");
            }

            ViewBag.UserId = userIdClaim;

            return View(recruitment);
        }
    }
}
