using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRN_Final_Project.API.Dto;
using PRN_Final_Project.Business.Entities;
using PRN_Final_Project.Repositories.Common;

namespace PRN_Final_Project.Repositories.Interface
{
    public interface ICVInfoRepository : ICommonRepository<CV_Info>
    {
       Task<List<CandidateDto>> FindCvInfoByRecruitmentId(int recruitmentId);
        Task ApproveCv(int cvId);
        Task RejectCv(int cvId);
        Task<int> CountActiveCVByRecruitmentId(int recruitmentId);
        Task<bool> ExistsByFileIdAndRecruitmentId(int fileId, int recruitmentId);
    }
}