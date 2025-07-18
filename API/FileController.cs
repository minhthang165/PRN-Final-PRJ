using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _service;
        private readonly IAIExtractor _extractor;

        public FileController(IFileService service, IAIExtractor extractor)
        {
            _service = service;
            _extractor = extractor;
        }

        [HttpPost]
        public async Task<IActionResult> UploadPdf(IFormFile file)
        {
            var uploadResult = await _service.UploadPdf(file);
            return Ok(new { message = "Uploaded", url = uploadResult.path });
        }

        // GET: api/file
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var files = await _service.GetAllAsync();
            return Ok(files);
        }

        // GET: api/file/paging?searchKey=abc&page=1&pageSize=10
        [HttpGet("paging")]
        public async Task<ActionResult> GetPaging([FromQuery] string? searchKey = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _service.GetAllPagingAsync(searchKey, page, pageSize);
            return Ok(pagedResult);
        }

        // GET: api/file/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var file = await _service.GetByIdAsync(id);
            if (file == null)
                return NotFound();
            return Ok(file);
        }

        // DELETE: api/file/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/file/user/5
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetByUserId(int id)
        {
            var file = await _service.GetByUserIdAsync(id);
            if (file == null)
                return NotFound();
            return Ok(file);
        }
    }
}