using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;

namespace PRN_Final_Project.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly PRNDbContext _context;

        public ClassRepository(PRNDbContext context)
        {
            _context = context;
        }

        public async Task<List<Class>> GetAllAsync()
        {
            return await _context.Classes.ToListAsync();
        }

        public async Task<Page<Class>> GetAllPagingAsync(string searchKey = "", int page = 1, int pageSize = 10)
        {
            var query = _context.Classes.AsQueryable();
            var totalItems = await query.CountAsync();
            var items = await query
                .Where(c => string.IsNullOrEmpty(searchKey) || c.class_name.Contains(searchKey))
                .OrderBy(c => c.class_name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Page<Class>
            {
                Items = items,
                TotalItems = totalItems,
                PageSize = pageSize,
                PageNumber = page,
            };
        }

        public async Task<Class> GetByIdAsync(int id)
        {
            return await _context.Classes
                .FirstOrDefaultAsync(c => c.id == id);
        }

        public async Task AddAsync(Class classes)
        {
            await _context.Classes.AddAsync(classes);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Class classes)
        {
            _context.Classes.Update(classes);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existedClass = await _context.Classes.FirstOrDefaultAsync(c => c.id == id);

            if (existedClass != null)
            {
                existedClass.is_active = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}