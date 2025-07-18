using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.Service
{
    public class ImportResult
    {
        public int SuccessCount { get; set; } = 0;
        public int FailedCount { get; set; } = 0;
        public List<string> Errors { get; set; } = new List<string>();
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            this._repository = repository;
        }

        public async Task AddAsync(user entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<List<user>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Page<user>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            return await _repository.GetAllPagingAsync(searchKey, page, pageSize);
        }

        public Task<user?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(user entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task<user> GetByEmail(string email)
        {
            return await _repository.GetByEmail(email);
        }

        public async Task<ImportResult> ImportEmployeesFromExcelAsync(IFormFile file)
        {
            var result = new ImportResult();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0);
                int rowCount = sheet.LastRowNum;

                for (int row = 1; row <= rowCount; row++)
                {
                    try
                    {
                        IRow excelRow = sheet.GetRow(row);
                        if (excelRow == null) continue;

                        var email = excelRow.GetCell(3)?.ToString();
                        var existingUsers = await _repository.GetAllAsync();
                        if (existingUsers.Any(u => u.email == email))
                        {
                            result.FailedCount++;
                            result.Errors.Add($"Row {row + 1}: Email '{email}' already exists.");
                            continue;
                        }
                        var user = new user
                        {
                            first_name = excelRow.GetCell(0)?.ToString(),
                            last_name = excelRow.GetCell(1)?.ToString(),
                            user_name = excelRow.GetCell(2)?.ToString(),
                            email = email,
                            phone_number = excelRow.GetCell(4)?.ToString(),
                            gender = excelRow.GetCell(5)?.ToString(),
                            role = "Employee",
                            created_at = System.DateTime.Now,
                            is_active = true
                        };
                        await _repository.AddAsync(user);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailedCount++;
                        result.Errors.Add($"Row {row + 1}: {ex.Message}");
                    }
                }
            }
            return result;
        }

        public async Task<List<user>> GetUsersByRoleAsync(string role)
        {
            return await _repository.GetUsersByRoleAsync(role);
        }

        public Task<Page<user>> GetUsersByRolePagingAsync(string role, string? searchKey = "", int page = 1, int pageSize = 10)
        {
            return _repository.GetUsersByRolePagingAsync(role, searchKey, page, pageSize);
        }
    }
}