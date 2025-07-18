using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Service;

namespace PRN_Final_Project.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _service;
        public EmailController(EmailService service)
        {
            _service = service;
        }

        [HttpPost("send-welcome-email/{userId}")]
        public async Task<IActionResult> SendWelcomeEmail(int userId)
        {
            try
            {
                await _service.SendWelcomeEmailAsync(userId);
                return Ok(new { message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}