using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;
using System.Security.Claims;

namespace PRN_Final_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        //Start Google Login
        [HttpGet("login/{provider}")]
        public IActionResult Login(string provider, string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/claim-role?returnUrl=" + Uri.EscapeDataString(returnUrl)
            };

            return Challenge(properties, provider);
        }

        //Handle Role Claim after Google login
        [Authorize]
        [HttpGet("claim-role")]
        public async Task<IActionResult> ClaimRole(string returnUrl = "/")
        {
            var user = HttpContext.User;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = user.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = user.FindFirst(ClaimTypes.Surname)?.Value;
            var picture = user.FindFirst("urn:google:picture")?.Value ?? "/assets/img/users/default-avatar.png";
            var phoneNumber = user.FindFirst(ClaimTypes.MobilePhone)?.Value;
            var gender = user.FindFirst(ClaimTypes.Gender)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Login");
            }

            // Check if user exists or create new
            var existingUser = await _userService.GetOneByEmail(email);
            if (existingUser == null)
            {
                var newUser = new user
                {
                    email = email,
                    first_name = firstName,
                    last_name = lastName,
                    avatar_path = picture,
                    phone_number = phoneNumber,
                    gender = gender,
                    role = "GUEST",
                    is_active = true,
                    created_at = DateTime.Now
                };

                await _userService.AddAsync(newUser);
                existingUser = await _userService.GetOneByEmail(email);
            }

            // Check if user is banned (exists in Redis cache)
            var banStatus = await _userService.GetBanStatus(existingUser.id);
            if (banStatus.ContainsKey("isBanned") && (bool)banStatus["isBanned"] == true)
            {
                // Add a TempData message to inform the user
                TempData["ErrorMessage"] = $"Your account has been banned. Reason: {banStatus["reason"]}";

                Redirect("/logout");
            }

            if (existingUser.is_active.HasValue && existingUser.is_active.Value == false)
            {
                Redirect("/logout");
            }

            // Create your own claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, existingUser.id.ToString()),
        new Claim(ClaimTypes.Email, existingUser.email),
        new Claim("Avatar", existingUser.avatar_path ?? "/assets/img/users/default-avatar.png"),
        new Claim(ClaimTypes.Name, existingUser.first_name + " " + existingUser.last_name),
        new Claim(ClaimTypes.Role, existingUser.role),
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(2)
            };


            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirect based on role
            if (existingUser.role == "ADMIN")
                return Redirect("/admin");
            else if (existingUser.role == "EMPLOYEE")
                return Redirect("/employee");
            else if (existingUser.role == "INTERN")
                return Redirect("/intern");
            else
                return Redirect("/landingpage");
        }
    }
}
