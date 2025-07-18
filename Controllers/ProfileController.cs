using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Controllers
{
    public class ProfileController : Controller
    {

        private readonly IUserService _userService;
        private readonly ICVInfoService _cvInfoService;
        private readonly IFileService _fileService;

        public ProfileController(IUserService userService, ICVInfoService cVInfoService, IFileService fileService)
        {
            _userService = userService;
            _cvInfoService = cVInfoService;
            _fileService = fileService;
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

        [HttpPost]
        public async Task<IActionResult> UploadCv(IFormFile cvFile)
        {
            if (cvFile == null || cvFile.Length == 0)
            {
                ModelState.AddModelError("", "Please select a file to upload.");
                return View("Index");
            }

            try
            {
                UserFile file = await _fileService.UploadPdf(cvFile);
                if (file == null)
                {
                    ModelState.AddModelError("", "File upload failed.");
                    return View("Index");
                }

                return RedirectToAction("Index", "Profile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index");
            }
        }
    }
}
