using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Message
{
    public int message_id { get; set; }

    public int? conversation_id { get; set; }

    public string? message_content { get; set; }

    public string? message_type { get; set; }

    public string? status { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public DateTime? deleted_at { get; set; }

    public int? created_by { get; set; }

    public int? updated_by { get; set; }

    public int? deleted_by { get; set; }

    public bool? is_active { get; set; }

    public virtual Conversation? conversation { get; set; }

    public virtual user? sender { get; set; }
}
