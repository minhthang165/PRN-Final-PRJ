using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Conversation_user
{
    public int user_id { get; set; }

    public int conversation_id { get; set; }

    public bool? is_admin { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }

    public virtual Conversation conversation { get; set; } = null!;

    public virtual user user { get; set; } = null!;
}
