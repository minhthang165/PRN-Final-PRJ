using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PRN_Final_Project.Controllers
{
    [Authorize]
    public class VideoCallController : Controller
    {
        public IActionResult Index(string room = null, string user = null)
        {
            // Get user ID from claims if not provided in query string
            if (string.IsNullOrEmpty(user))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? User.FindFirst("UserId")?.Value;
                user = userIdClaim;
            }

            // Validate required parameters
            if (string.IsNullOrEmpty(room))
            {
                TempData["ErrorMessage"] = "Invalid room ID. Please use a valid call link.";
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(user))
            {
                TempData["ErrorMessage"] = "Please log in to join the video call.";
                return RedirectToAction("Index", "Login");
            }

            // Pass data to view if needed
            ViewData["RoomId"] = room;
            ViewData["UserId"] = user;

            return View();
        }

        [HttpGet]
        [Route("/video-call")]
        public IActionResult VideoCallRedirect(string room = null, string user = null)
        {
            return RedirectToAction("Index", new { room, user });
        }
    }
}