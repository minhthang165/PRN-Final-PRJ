using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Controllers
{
    public class ProfileController : Controller
    {

        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        } 

        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return View(_userService.GetByIdAsync(Convert.ToInt32(userIdClaim)).Result);
        }
        
        [HttpGet]
        public IActionResult Edit()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return View(_userService.GetByIdAsync(Convert.ToInt32(userIdClaim)).Result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(user model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _userService.UpdateAsync(model);
            return RedirectToAction("Index", "Profile");
        }
    }
}
