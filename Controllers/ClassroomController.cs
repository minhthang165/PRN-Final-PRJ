using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PRN_Final_Project.Service.Interface;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PRN_Final_Project.Controllers
{
    [Authorize]
    [Route("Class")] // Add this route to match the URL
    public class ClassroomController : Controller
    {
        private readonly IClassService _classService;
        private readonly IUserService _userService;

        public ClassroomController(IClassService classService, IUserService userService)
        {
            _classService = classService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string role = null, int page = 1, int pageSize = 10)
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

            // Load users with EMPLOYEE role for manager dropdown (for ADMIN only)
            if (userRoleClaim == "ADMIN")
            {
                try
                {
                    var employees = await _userService.GetUsersByRoleAsync("EMPLOYEE");
                    ViewBag.userRoleList = employees;
                }
                catch (Exception ex)
                {
                    // Log error and provide empty list
                    ViewBag.userRoleList = new List<Business.Entities.user>();
                }
            }

            // For INTERN role, fetch their class data
            Business.Entities.Class classData = null;
            if (userRoleClaim == "INTERN")
            {
                try
                {
                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        classData = await _classService.GetClassByUserId(userId);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue - the view will handle null model
                    Console.WriteLine($"Error fetching class for intern: {ex.Message}");
                }
            }

            // Return appropriate view based on user role
            return userRoleClaim switch
            {
                "ADMIN" => View("~/Views/Admin/ManageClass.cshtml"),
                "EMPLOYEE" => View("~/Views/Employee/manage-class.cshtml"),
                "MANAGER" => View("~/Views/Employee/manage-class.cshtml"), // MANAGER uses the same view as EMPLOYEE
                "INTERN" => View("~/Views/Intern/InternViewClass.cshtml", classData),
                _ => View("~/Views/Shared/Error.cshtml")
            };
        }
    }
}