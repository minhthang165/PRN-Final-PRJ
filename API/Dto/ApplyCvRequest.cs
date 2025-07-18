using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.API.Dto
{
    public class ApplyCvRequest
    {
        public int FileId { get; set; }
        public int RecruitmentId { get; set; }
    }
}