using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;

namespace PRN_Final_Project.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly PRNDbContext _context;
        public RoomRepository(PRNDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Room entity)
        {
            await _context.Rooms.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            var existingRoom = _context.Rooms.Find(id);
            if (existingRoom != null)
            {
                existingRoom.is_active = false; // Assuming IsActive is a property to mark as deleted
                return _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Room with ID {id} not found.");
            }
        }

        public Task<List<Room>> GetAllAsync()
        {
            return _context.Rooms.ToListAsync();
        }

        public Task<Page<Room>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            var query = _context.Rooms.AsQueryable();
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(r => r.room_name.Contains(searchKey));
            }
            var totalItems = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return Task.FromResult(new Page<Room>
            {
                SearchTerm = searchKey,
                Items = items.Result,
                TotalItems = totalItems,
                PageSize = pageSize,
                PageNumber = page
            });
        }

        public Task<Room?> GetByIdAsync(int id)
        {
            return _context.Rooms.FirstOrDefaultAsync(r => r.id == id);
        }

        public Task UpdateAsync(Room entity)
        {
            var existingRoom = _context.Rooms.Find(entity.id);
            if (existingRoom != null)
            {
                existingRoom.room_name = entity.room_name;
                existingRoom.capicity = entity.capicity;
                existingRoom.is_active = entity.is_active; // Assuming IsActive is a property to mark as active/inactive
                return _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Room with ID {entity.id} not found");
            }
        }
    }
}
