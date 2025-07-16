using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.Service.Dto
{
    public class CVExtractedInfo
    {
        public decimal? GPA { get; set; }
        public List<string> Education { get; set; }
        public List<string> Skill { get; set; }
    }
}