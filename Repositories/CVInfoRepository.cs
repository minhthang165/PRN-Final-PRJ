using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.API.Dto;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Repositories
{
    public class CVInfoRepository : ICVInfoRepository
    {
        private readonly PRNDbContext _context;
        private readonly IFileRepository _file;
        private readonly EmailService _emailService;

        public CVInfoRepository(PRNDbContext context, IFileRepository file, EmailService emailService)
        {
            _file = file;
            _context = context;
            _emailService = emailService;
        }

        public async Task<List<CV_Info>> GetAllAsync()
        {
            return await _context.CV_Infos
                .ToListAsync();
        }

        public async Task<Page<CV_Info>> GetAllPagingAsync(string searchKey = "", int page = 1, int pageSize = 10)
        {
            var query = _context.CV_Infos.AsQueryable();
            var totalItems = await query.CountAsync();
            var items = await query
                .Where(c => string.IsNullOrEmpty(searchKey) || c.education.Contains(searchKey))
                .Where(c => c.is_active == true)
                .OrderBy(c => c.file_id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Page<CV_Info>
            {
                Items = items,
                TotalItems = totalItems,
                PageSize = pageSize,
                PageNumber = page,
            };
        }

        public async Task<CV_Info> GetByIdAsync(int id)
        {
            return await _context.CV_Infos
                .FirstOrDefaultAsync(c => c.file_id == id);
        }

        public async Task AddAsync(CV_Info cv_info)
        {
            await _context.CV_Infos.AddAsync(cv_info);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(CV_Info cv_info)
        {
            _context.CV_Infos.Update(cv_info);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existedCV = await _context.CV_Infos.FirstOrDefaultAsync(c => c.file_id == id);

            if (existedCV != null)
            {
                existedCV.is_active = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CandidateDto>> FindCvInfoByRecruitmentId(int recruitmentId)
        {
            var candidates = await _context.CV_Infos
                .Where(ci => ci.recruitment_id == recruitmentId && ci.is_active == true)
                .Include(ci => ci.file)
                    .ThenInclude(f => f.submitter)
                .Select(ci => new CandidateDto
                {
                    first_name = ci.file.submitter.first_name,
                    last_name = ci.file.submitter.last_name,
                    gpa = ci.gpa,
                    education = ci.education,
                    skill = ci.skill,
                    path = ci.file.path,
                    fileId = ci.file_id,
                    isActive = ci.is_active
                })
            .ToListAsync();
            return candidates;
        }

        public async Task ApproveCv(int cvId)
        {
            // 1. Tìm CV
            CV_Info cv = await _context.CV_Infos
                .FirstOrDefaultAsync(c => c.cvInfo_id == cvId);
            if (cv == null)
                throw new Exception("CV not found");

            // 2. Lấy thông tin class từ recruiment
            Recruitment recruitment = await _context.Recruitments
                .FirstOrDefaultAsync(c => c.id == cv.recruitment_id);
            if (recruitment == null)
                throw new Exception("Recruitment not found");

            Class classes = _context.Classes
                .FirstOrDefault(c => c.id == recruitment.class_id);
            if (classes == null)
                throw new Exception("Class not found");

            // 3. Lấy thông tin user từ file thông qua submitterId
            UserFile file = await _context.UserFiles.
                FirstOrDefaultAsync(c => c.id == cv.file_id);
            if (file == null)
                throw new Exception("File not found");

            user user = await _context.users
                .FirstOrDefaultAsync(c => c.id == file.submitter_id);
            if (user == null)
                throw new Exception("User not found");

            // 4.Check user đã có trong lớp chưa
            if (user.class_id != null)
            {
                throw new Exception("User are in another class");
            }

            // 5. Cập nhật role thành Intern
            user.role = "INTERN";

            // 6. Thêm user vào lớp học
            user.class_id = classes.id;

            // 7. Cập nhật số lượng intern trong lớp đó
            classes.number_of_interns += 1;

            // 8. Đánh dấu cv đã đc approve (=> inActive = false)
            cv.is_active = false;

            await _emailService.SendWelcomeEmailAsync(user.id);
            await _context.SaveChangesAsync();
        }

        public async Task RejectCv(int cvId)
        {
            CV_Info cv = await _context.CV_Infos
               .FirstOrDefaultAsync(c => c.cvInfo_id == cvId);
            if (cv == null)
                throw new Exception("CV not found");

            UserFile file = await _context.UserFiles
                .FirstOrDefaultAsync(c => c.id == cv.file_id);
            if (file == null)
                throw new Exception("File not found");
            user user = await _context.users
                .FirstOrDefaultAsync(c => c.id == file.submitter_id);
            if (user == null)
                throw new Exception("User not found");

            cv.is_active = false;
            // Gửi email thông báo từ chối CV
            await _emailService.SendRejectEmailAsync(user.id);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountActiveCVByRecruitmentId(int recruitmentId)
        {
            return await _context.CV_Infos
                .CountAsync(c => c.is_active == true && c.recruitment_id == recruitmentId);
        }

        public async Task<bool> ExistsByFileIdAndRecruitmentId(int fileId, int recruitmentId)
        {
            return await _context.CV_Infos
                .AnyAsync(c => c.file_id == fileId && c.recruitment_id == recruitmentId && c.is_active == true);
        }
    }
}