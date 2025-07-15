using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN_Final_Project.Repositories.Common;

namespace PRN_Final_Project.Service.Common
{
    public interface ICommonService<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<Page<T>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}