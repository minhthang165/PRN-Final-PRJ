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

        public async Task<IActionResult> Index(string role = null, int page = 1, int pageSize = 10)
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = Convert.ToInt32(userIdClaim);

            // Check user role and return appropriate view
            if (userRoleClaim == "ADMIN")
            {
                try
                {
                    var employeeUsers = await _userService.GetAllPagingAsync("EMPLOYEE", page, pageSize);
                    ViewData["userRoleList"] = employeeUsers;
                    return View("~/Views/Admin/manage-class.cshtml");
                }
                catch (Exception ex)
                {
                    ViewData["error"] = "Class list is empty";
                    return View("~/Views/Admin/Index.cshtml");
                }
            }
            else if (userRoleClaim == "EMPLOYEE")
            {
                try
                {
                    var internUsers = await _userService.GetAllPagingAsync("INTERN", page, pageSize);
                    var classes = await _classService.GetClassesByMentorId(userId);
                    
                    ViewData["internClassList"] = internUsers;
                    ViewData["classroomList"] = classes;
                    ViewData["mentorId"] = userId;
                    
                    return View("~/Views/Employee/manage-class.cshtml");
                }
                catch (Exception ex)
                {
                    ViewData["error"] = "Class list is empty";
                    return View("~/Views/Employee/Index.cshtml");
                }
            }
            else if (userRoleClaim == "INTERN")
            {
                try
                {
                    var user = await _userService.GetByIdAsync(userId);
                    var internUsers = await _userService.GetAllPagingAsync("INTERN", page, pageSize);
                    var classes = await _classService.GetClassesByMentorId(userId);
                    
                    ViewData["internClassList"] = internUsers;
                    ViewData["classroomList"] = classes;
                    ViewData["classId"] = user.class_id;
                    ViewData["internId"] = userId;
                    
                    return View("~/Views/Intern/InternViewClass.cshtml");
                }
                catch (Exception ex)
                {
                    ViewData["error"] = "Class list is empty";
                    return View("~/Views/Intern/Index.cshtml");
                }
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }
    }
}