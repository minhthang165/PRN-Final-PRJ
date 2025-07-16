using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;

namespace PRN_Final_Project.Repositories
{
    public class RecruitmentRepository : IRecruitmentRepository
    {
        private readonly PRNDbContext _context;

        public RecruitmentRepository(PRNDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recruitment>> GetAllAsync()
        {
            return await _context.Recruitments.ToListAsync();
        }

        public async Task<Page<Recruitment>> GetAllPagingAsync(string searchKey = "", int page = 1, int pageSize = 10)
        {
            var query = _context.Recruitments.AsQueryable();
            var totalItems = await query.CountAsync();
            var items = await query
                .Where(c => string.IsNullOrEmpty(searchKey) || c.name.Contains(searchKey))
                .OrderBy(c => c.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Page<Recruitment>
            {
                Items = items,
                TotalItems = totalItems,
                PageSize = pageSize,
                PageNumber = page,
            };
        }

        public async Task<Recruitment> GetByIdAsync(int id)
        {
            return await _context.Recruitments
                .FirstOrDefaultAsync(c => c.id == id);
        }

        public async Task AddAsync(Recruitment recruitment)
        {
            await _context.Recruitments.AddAsync(recruitment);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Recruitment recruitment)
        {
            _context.Recruitments.Update(recruitment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existedClass = await _context.Recruitments.FirstOrDefaultAsync(c => c.id == id);

            if (existedClass != null)
            {
                existedClass.is_active = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}