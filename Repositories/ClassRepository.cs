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
            return await _context.Classes.Include(c => c.mentor).ToListAsync();
        }

        public async Task<Page<Class>> GetAllPagingAsync(string searchKey = "", int page = 1, int pageSize = 10)
        {
            var query =  _context.Classes.Include(c => c.mentor).AsQueryable();
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
            return await _context.Classes.Include(c => c.mentor)
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

        public async Task AssignTrainerToClassAsync(int classId, int trainerId)
        {
            var trainer = await _context.users.FindAsync(trainerId);
            if (trainer == null || (trainer.role != "EMPLOYEE"))
                throw new Exception("User is not a valid trainer");

            var classObj = await _context.Classes.FindAsync(classId);
            if (classObj == null)
                throw new Exception("Class not found");

            classObj.mentor_id = trainerId;
            await _context.SaveChangesAsync();
        }

        public async Task AssignTraineeToClassAsync(int classId, int traineeId)
        {
            var trainee = await _context.users.FindAsync(traineeId);
            if (trainee == null || (trainee.role != "INTERN"))
                throw new Exception("User is not a valid trainee");

            var classObj = await _context.Classes.FindAsync(classId);
            if (classObj == null)
                throw new Exception("Class not found");

            trainee.class_id = classObj.id;
            await _context.SaveChangesAsync();
        }

        public Task<List<Class>> GetClassesByMentorId(int userId)
        {
            var classes = _context.Classes
                .Where(c => c.mentor_id == userId)
                .ToListAsync();
            return classes;
        }
    }
}