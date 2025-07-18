using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PRN_Final_Project.API.Dto;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class CVInfoController : ControllerBase
    {
        private readonly ICVInfoService _service;

        public CVInfoController(ICVInfoService service)
        {
            _service = service;
        }

        [HttpPost("upload-cv")]
        public async Task<IActionResult> ApplyCV([FromBody] ApplyCvRequest request)
        {
            CV_Info cvInfo = await _service.ApplyCv(request.FileId, request.RecruitmentId);

            var resultDto = new CVInfoResultDto
            {
                Id = cvInfo.cvInfo_id,
                Gpa = cvInfo.gpa ?? 0,
                Education = cvInfo.education,
                Skill = cvInfo.skill,
                CreatedAt = cvInfo.created_at,
                File = new UserFileDto
                {
                    Id = cvInfo.file_id,
                    DisplayName = cvInfo.file.display_name,
                    Path = cvInfo.file.path
                }
            };

            return Ok(new
            {
                message = "Upload & extract thành công",
                data = resultDto
            });
        }

        // GET: api/CVInfo
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cvs = await _service.GetAllAsync();
            return Ok(cvs);
        }

        // GET: api/CVInfo/paging?searchKey=&page=1&pageSize=10
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging([FromQuery] string? searchKey = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllPagingAsync(searchKey, page, pageSize);
            return Ok(result);
        }

        // GET: api/CVInfo/id/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cv = await _service.GetByIdAsync(id);
            if (cv == null)
                return NotFound();
            return Ok(cv);
        }

        // PUT: api/CVInfo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CV_Info cv)
        {
            if (id != cv.file_id)
                return BadRequest("ID mismatch");

            await _service.UpdateAsync(cv);
            return NoContent();
        }

        // DELETE: api/CVInfo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/CVInfo/recruitment/123
        [HttpGet("recruitment/{recruitmentId}")]
        public async Task<IActionResult> GetCvInfoByRecruitmentId(int recruitmentId)
        {
            var cvs = await _service.FindCvInfoByRecruitmentId(recruitmentId);
            return Ok(cvs);
        }

        [HttpPost("approve-cv")]
        public async Task<IActionResult> ApproveCv(int cvId)
        {
            await _service.ApproveCv(cvId);
            // 5. Trả về response
            return Ok(new
            {
                message = "Chấp thuận cv thành công",
            });
        }

        [HttpPost("reject-cv")]
        public async Task<IActionResult> RejectCv(int cvId)
        {
            await _service.RejectCv(cvId);
            // 5. Trả về response
            return Ok(new
            {
                message = "Từ chối cv thành công",
            });
        }

        [HttpGet("count-active-cv/{recruitmentId}")]
        public async Task<IActionResult> CountActiveCVByRecruitmentId(int recruitmentId)
        {
            int count = await _service.CountActiveCVByRecruitmentId(recruitmentId);
            return Ok(new
            {
                message = "Count active CVs by recruitment ID",
                count = count
            });
        }

        [HttpGet("exists-by-file-and-recruitment")]
        public async Task<IActionResult> ExistsByFileIdAndRecruitmentId(int fileId, int recruitmentId)
        {
            bool exists = await _service.ExistsByFileIdAndRecruitmentId(fileId, recruitmentId);
            return Ok(new
            {
                message = "Check if CV exists by file and recruitment ID",
                exists = exists
            });
        }
    }
}