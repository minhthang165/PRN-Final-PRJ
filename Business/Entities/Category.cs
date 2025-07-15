using System;
using System.Collections.Generic;

namespace PRN_Final_Project.Business.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int? ParentCategoryId { get; set; }

    public bool SubCategoryStatus { get; set; }

    public bool Status { get; set; }

    public bool ShowMenuStatus { get; set; }
}
