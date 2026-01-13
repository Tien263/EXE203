using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Role { get; set; }

    public int? EmployeeId { get; set; }

    public int? CustomerId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastLoginDate { get; set; }
    
    public string? ProfileImageUrl { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }
}
