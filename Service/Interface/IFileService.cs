using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Service.Common;

namespace PRN_Final_Project.Service.Interface
{
    public interface IFileService : ICommonService<UserFile>
    {
        Task<UserFile> UploadPdf(IFormFile file);
        Task<List<UserFile>> GetByUserIdAsync(int id);
    }
}