
using System.Security.Claims;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly Cloudinary _cloudinary;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PRNDbContext _context;


        public FileRepository(Cloudinary cloudinary, PRNDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _cloudinary = cloudinary;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : (int?)null;
        }

        public async Task<UserFile> UploadPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded");

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "Uploaded_CVs",
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true,
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception("Upload error: " + uploadResult.Error.Message);

            var userId = GetCurrentUserId();
            if (userId == null)
                throw new Exception("User is not authenticated.");

            var saveFile = new UserFile
            {
                submitter_id = userId.Value,
                display_name = file.FileName,
                path = uploadResult.SecureUrl.AbsoluteUri,
                created_at = DateTime.Now,
                is_active = true
            };

            _context.UserFiles.Add(saveFile);
            await _context.SaveChangesAsync();
            return saveFile;
        }

        public async Task<List<UserFile>> GetAllAsync()
        {
            return await _context.UserFiles.ToListAsync();
        }

        public async Task<Page<UserFile>> GetAllPagingAsync(string searchKey = "", int page = 1, int pageSize = 10)
        {
            var query = _context.UserFiles.AsQueryable();
            var totalItems = await query.CountAsync();
            var items = await query
                .Where(c => string.IsNullOrEmpty(searchKey) || c.display_name.Contains(searchKey))
                .OrderBy(c => c.display_name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Page<UserFile>
            {
                Items = items,
                TotalItems = totalItems,
                PageSize = pageSize,
                PageNumber = page,
            };
        }

        public async Task<UserFile> GetByIdAsync(int id)
        {
            return await _context.UserFiles
                .FirstOrDefaultAsync(c => c.id == id);
        }

        public async Task AddAsync(UserFile userFile)
        {
            await _context.UserFiles.AddAsync(userFile);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(UserFile file)
        {
            _context.UserFiles.Update(file);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existedFile = await _context.UserFiles.FirstOrDefaultAsync(c => c.id == id);

            if (existedFile != null)
            {
                _context.UserFiles.Remove(existedFile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<UserFile>> GetByUserIdAsync(int id)
        {
            return await _context.UserFiles
                .Where(c => c.submitter_id == id && c.is_active == true)
                .ToListAsync();
        }
    }
}