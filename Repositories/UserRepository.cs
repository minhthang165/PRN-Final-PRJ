using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PRN_Final_Project.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly IDistributedCache _cache;
        private const string BAN_PREFIX = "user:ban:";
        private readonly PRNDbContext _context;

        public UserRepository(PRNDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
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
            return await _context.users
                .ToListAsync();
        }

        public async Task<Page<user>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            var query = _context.users.AsQueryable();

            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(u => u.role.Contains(searchKey));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.first_name)
                .Take(pageSize)
                .ToListAsync();

            foreach (var user in items)
            {
                var banKey = BAN_PREFIX + user.id;
                var banInfoJson = await _cache.GetStringAsync(banKey);

                if (!string.IsNullOrEmpty(banInfoJson))
                {
                    user.is_active = false; // Mark as banned in the result
                }
            }

            return new Page<user>
            {
                Items = items,
                TotalItems = totalItems,
                PageSize = pageSize,
                PageNumber = page
            };
        }


        public async Task<user?> GetByEmail(string email)
        {
            try
            {
                return await _context.users
                     .FirstOrDefaultAsync(u => u.email == email);
            }
            catch
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
        public async Task BanUser(int userId, int durationInDays, string reason)
        {
            // Update user active status in database
            var user = await _context.users.FindAsync(userId);
            if (user != null)
            {
                user.is_active = false;
                user.updated_at = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            // Store ban information in cache
            var banInfo = new Dictionary<string, object>
            {
                ["userId"] = userId,
                ["isBanned"] = true,
                ["duration"] = durationInDays,
                ["reason"] = reason,
                ["bannedUntil"] = DateTime.Now.AddDays(durationInDays)
            };

            var banInfoJson = JsonSerializer.Serialize(banInfo);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(durationInDays)
            };

            await _cache.SetStringAsync(
                BAN_PREFIX + userId,
                banInfoJson,
                cacheOptions
            );
        }

        public async Task<Dictionary<string, object>> GetBanStatus(int userId)
        {
            var banInfoJson = await _cache.GetStringAsync(BAN_PREFIX + userId);

            if (string.IsNullOrEmpty(banInfoJson))
            {
                return new Dictionary<string, object>
                {
                    ["userId"] = userId,
                    ["isBanned"] = false
                };
            }

            var banInfo = JsonSerializer.Deserialize<Dictionary<string, object>>(banInfoJson);

            // Calculate remaining time
            var remainingTime = _cache.GetString(BAN_PREFIX + userId);
            if (remainingTime != null)
            {
                var expiryTime = await _cache.GetStringAsync(BAN_PREFIX + userId + ":expiry");
                if (!string.IsNullOrEmpty(expiryTime) && DateTime.TryParse(expiryTime, out DateTime expiry))
                {
                    banInfo["remainingDuration"] = (expiry - DateTime.Now).TotalSeconds;
                }
            }

            return banInfo;
        }

        public async Task UnbanUser(int userId)
        {
            // Update user active status in database
            var user = await _context.users.FindAsync(userId);
            if (user != null)
            {
                user.is_active = true;
                user.updated_at = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            // Remove ban information from cache
            await _cache.RemoveAsync(BAN_PREFIX + userId);
            await _cache.RemoveAsync(BAN_PREFIX + userId + ":expiry");
        }
    }
}