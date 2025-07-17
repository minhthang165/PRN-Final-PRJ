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
            return View();
        }
        
        [HttpGet]
        public IActionResult Edit()
        {
            var user = _userService.GetByIdAsync(Convert.ToInt32(User.FindFirst("UserId")?.Value)).Result;
            return View(user);
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
