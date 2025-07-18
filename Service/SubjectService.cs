using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;
using PRN_Final_Project.Repositories.Interface;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Service
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _repository;
        public SubjectService(ISubjectRepository repository)
        {
            _repository = repository;
        }
        public Task AddAsync(Subject entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Subject entity cannot be null");
            }
            return _repository.AddAsync(entity);
        }

        public Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid subject ID", nameof(id));
            }
            return _repository.DeleteAsync(id);
        }

        public Task<List<Subject>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<Page<Subject>> GetAllPagingAsync(string? searchKey = "", int page = 1, int pageSize = 10)
        {
            if (page < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than or equal to 1");
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than or equal to 1");
            }
            return _repository.GetAllPagingAsync(searchKey, page, pageSize);
        }

        public Task<Subject?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid subject ID", nameof(id));
            }
            return _repository.GetByIdAsync(id);
        }

        public Task UpdateAsync(Subject entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Subject entity cannot be null");
            }
            if (entity.id <= 0)
            {
                throw new ArgumentException("Invalid subject ID", nameof(entity.id));
            }
            return _repository.UpdateAsync(entity);
        }
    }
}
