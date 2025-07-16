using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PRNDbContext _context;

        public UserRepository(PRNDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(user entity)
        {
            await _context.users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ExistingUser = await _context.users.FindAsync(id);
            if (ExistingUser != null)
            {
                ExistingUser.is_active = false;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }
        }

        public async Task<List<user>> GetAllAsync()
        {
            return await _context.users.ToListAsync();
        }

        public Task<Page<user>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public async Task<user?> GetByEmail(string email)
        {
            try
            {
                return await _context.users.FirstOrDefaultAsync(u => u.email == email);
            } catch
            {
                return null;
            }
        }

        public async Task<user?> GetByIdAsync(int id)
        {
            return await _context.users
                .FirstOrDefaultAsync(u => u.id == id);
        }

        public Task UpdateAsync(user entity)
        {
            var existingUser = _context.users.Find(entity.id);
            if (existingUser != null)
            {
                if (entity.first_name != null)
                    existingUser.first_name = entity.first_name;

                if (entity.last_name != null)
                    existingUser.last_name = entity.last_name;

                if (entity.avatar_path != null)
                    existingUser.avatar_path = entity.avatar_path;

                if (entity.phone_number != null)
                    existingUser.phone_number = entity.phone_number;

                if (entity.class_id != null)
                    existingUser.class_id = entity.class_id;

                if (entity.role != null)
                    existingUser.role = entity.role;

                if (entity.is_active != null)
                    existingUser.is_active = entity.is_active;

                return _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"User with ID {entity.id} not found");
            }

        }
    }
}