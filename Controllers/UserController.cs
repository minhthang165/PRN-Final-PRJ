using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Service.Interface;
using System.Security.Claims;

namespace PRN_Final_Project.Controllers
{
    [Authorize]
    [Route("admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index(string role = null)
        {
            // Get user role from claims
            var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userRoleClaim))
            {
                return RedirectToAction("Login", "Login");
            }

            if (userRoleClaim == "ADMIN")
            {
                if (role == "intern")
                {
                    ViewData["selectedRole"] = role;
                    return View("~/Views/Admin/ManageIntern.cshtml");
                }
                else if (role == "employee")
                {
                    ViewData["selectedRole"] = role;
                    return View("~/Views/Admin/ManageEmployee.cshtml");
                }
                return View("~/Views/Admin/Index.cshtml");
            }
            else if (userRoleClaim == "EMPLOYEE")
            {
                return View("~/Views/Employee/Index.cshtml");
            }
            else if (userRoleClaim == "INTERN")
            {
                return View("~/Views/Intern/Index.cshtml");
            }

            return View("~/Views/Shared/Error.cshtml");
        }

        [HttpGet("current-user-id")]
        public IActionResult GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Content(userId ?? string.Empty);
        }
    }
}