using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.Service.Interface
{
    public interface IUserService : ICommonService<user>
    {
        Task<user> GetByEmail(string email);
        Task<ImportResult> ImportEmployeesFromExcelAsync(IFormFile file);
        Task<List<user>> GetUsersByRoleAsync(string role);
        Task<Page<user>> GetUsersByRolePagingAsync(string role, string? searchKey = "", int page = 1, int pageSize = 10);
    }
}