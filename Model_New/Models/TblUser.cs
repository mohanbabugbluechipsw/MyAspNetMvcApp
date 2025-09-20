using System;
using System.Collections.Generic;

namespace Model_New.Models;

public partial class TblUser
{
    public int UserId { get; set; }

    public string EmpNo { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? EmpName { get; set; }

    public string? EmpEmail { get; set; }

    public DateTime? HireDate { get; set; }

    public string? LineManager { get; set; }

    public int LocationId { get; set; }

    public bool? IsActive { get; set; }

    public string? Gender { get; set; }

    public string? LineManagerEmail { get; set; }

    public string? UserName { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? UserTypeId { get; set; }

    // Navigation properties
    public virtual TblOfficeLocation Location { get; set; } = null!;

    public virtual TblUserType? UserType { get; set; }
}
