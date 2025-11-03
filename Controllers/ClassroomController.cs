using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PRN_Final_Project.Service.Interface;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PRN_Final_Project.Controllers
{
    [Authorize]
    public class ClassroomController : Controller
    {
        private readonly IClassService _classService;
        private readonly IUserService _userService;

        public ClassroomController(IClassService classService, IUserService userService)
        {
            _classService = classService;
            _userService = userService;
        }

        public IActionResult Index(string role = null, int page = 1, int pageSize = 10)
        {
            // Get user ID and role from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
            {
                return RedirectToAction("Login", "Login");
            }

            // Pass user information to the view for JavaScript to use
            ViewData["UserId"] = userIdClaim;
            ViewData["UserRole"] = userRoleClaim;
            ViewData["PageSize"] = pageSize;
            ViewData["CurrentPage"] = page;

            // Return appropriate view based on user role
            return userRoleClaim switch
            {
                "ADMIN" => View("~/Views/Admin/manage-class.cshtml"),
                "EMPLOYEE" => View("~/Views/Employee/manage-class.cshtml"),
                "INTERN" => View("~/Views/Intern/InternViewClass.cshtml"),
                _ => View("~/Views/Shared/Error.cshtml")
            };
        }
    }
}