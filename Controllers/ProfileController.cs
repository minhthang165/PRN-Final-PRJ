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
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Please select a file to upload." });
                }
                ModelState.AddModelError("", "Please select a file to upload.");
                return View("Index");
            }

            try
            {
                UserFile file = await _fileService.UploadPdf(cvFile);
                if (file == null)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "File upload failed." });
                    }
                    ModelState.AddModelError("", "File upload failed.");
                    return View("Index");
                }

                // Return JSON response for AJAX requests
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { 
                        success = true, 
                        message = "File uploaded successfully!",
                        data = new {
                            id = file.id,
                            display_name = file.display_name,
                            url = file.path,
                            path = file.path
                        }
                    });
                }

                return RedirectToAction("Index", "Profile");
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = ex.Message });
                }
                ModelState.AddModelError("", ex.Message);
                return View("Index");
            }
        }
    }
}
