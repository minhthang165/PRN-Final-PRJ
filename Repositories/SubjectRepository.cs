using Microsoft.EntityFrameworkCore;
using PRN_Final_Project.Business.Data;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;

namespace PRN_Final_Project.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly PRNDbContext pRNDbContext;
        public SubjectRepository(PRNDbContext pRNDbContext)
        {
            this.pRNDbContext = pRNDbContext;
        }
        public async Task AddAsync(Subject entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Subject entity cannot be null.");
            }
            await pRNDbContext.Subjects.AddAsync(entity);
            await pRNDbContext.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            var subject = pRNDbContext.Subjects.Find(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {id} not found.");
            }
            pRNDbContext.Subjects.Remove(subject);
            return pRNDbContext.SaveChangesAsync();
        }

        public Task<List<Subject>> GetAllAsync()
        {
            return pRNDbContext.Subjects.ToListAsync();
        }

        public Task<Page<Subject>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            var query = pRNDbContext.Subjects.AsQueryable();
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(s => s.subject_name.Contains(searchKey));
            }
            var totalItems = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return Task.FromResult(new Page<Subject>
            {
                SearchTerm = searchKey,
                Items = items.Result,
                TotalItems = totalItems,
                PageSize = pageSize,
                PageNumber = page
            });
        }

        public Task<Subject?> GetByIdAsync(int id)
        {
            return pRNDbContext.Subjects.FindAsync(id).AsTask();
        }

        public Task UpdateAsync(Subject entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Subject entity cannot be null.");
            }
            var existingSubject = pRNDbContext.Subjects.Find(entity.id);
            if (existingSubject == null)
            {
                throw new KeyNotFoundException($"Subject with ID {entity.id} not found");
            }
            existingSubject.subject_name = entity.subject_name;
            existingSubject.description = entity.description;
            existingSubject.is_active = entity.is_active;
            pRNDbContext.Subjects.Update(existingSubject);
            return pRNDbContext.SaveChangesAsync();
        }
    }
}
