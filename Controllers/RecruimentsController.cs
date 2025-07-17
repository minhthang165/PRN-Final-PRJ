using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Service.Interface;
using System.Threading.Tasks;

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
            return View();
        }

        // GET: /Recruitment/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var recruitment = await _recruitmentService.GetByIdAsync(id);

            if (recruitment == null)
            {
                return RedirectToAction("Index", "Recruitments");
            }

            return View(recruitment);
        }
    }
}
