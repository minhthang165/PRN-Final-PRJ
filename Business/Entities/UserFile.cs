using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class UserFile
{
    public int id { get; set; }

    public int submitter_id { get; set; }

    public string display_name { get; set; } = null!;

    public string path { get; set; } = null!;

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }

    public virtual ICollection<CV_Info> CV_Infos { get; set; } = new List<CV_Info>();

    public virtual user submitter { get; set; } = null!;
}
