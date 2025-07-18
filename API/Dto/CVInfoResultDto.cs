using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Final_Project.API.Dto
{
    public class CVInfoResultDto
    {
        public int Id { get; set; }
        public decimal? Gpa { get; set; }
        public string Education { get; set; }
        public string Skill { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserFileDto File { get; set; }
    }
}