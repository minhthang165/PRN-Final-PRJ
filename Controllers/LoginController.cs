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
            var picture = user.FindFirst("picture")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Login");
            }

            // Check if user exists or create new
            var existingUser = await _userService.GetByEmail(email);
            if (existingUser == null)
            {
                var newUser = new user
                {
                    email = email,
                    first_name = firstName,
                    last_name = lastName,
                    avatar_path = picture,
                    role = "GUEST"
                };

                await _userService.AddAsync(newUser);
                existingUser = newUser;
            }

            // Create your own claims
            var claims = new List<Claim>    
    {
        new Claim(ClaimTypes.NameIdentifier, existingUser.id.ToString()),
        new Claim(ClaimTypes.Email, existingUser.email),
        new Claim("Avatar", existingUser.avatar_path ?? "/assets/img/default-avatar.png"),
        new Claim(ClaimTypes.Name, existingUser.first_name + " " + existingUser.last_name),
        new Claim(ClaimTypes.Role, existingUser.role)
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
                return Redirect("/manage-user");
            else if (existingUser.role == "EMPLOYEE" || existingUser.role == "INTERN")
                return Redirect("/manageclass");
            else
                return Redirect("/landingpage");
        }
    }
}
