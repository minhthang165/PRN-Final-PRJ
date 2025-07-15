using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.Repositories.Common
{
    public interface ICommonRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<Page<T>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}