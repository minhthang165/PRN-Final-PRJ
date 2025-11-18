using System;

namespace PRN_Final_Project.API.Dto
{
    public class CreateRecruitmentDto
    {
        public string name { get; set; } = null!;
        public string position { get; set; } = null!;
        public string experience_requirement { get; set; } = null!;
        public string? language { get; set; }
        public decimal? min_GPA { get; set; }
        public int total_slot { get; set; }
        public string? description { get; set; }
        public DateTime end_time { get; set; }
        public int class_id { get; set; }
        public bool? is_active { get; set; }
    }
}
