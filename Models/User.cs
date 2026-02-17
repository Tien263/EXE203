using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
namespace Exe_Demo.Models;
// Kế thừa IdentityUser<int> để dùng Id có sẵn (chuẩn Identity)
public partial class User : IdentityUser<int>
{
    // Đã xóa public int UserId cũ để tránh xung đột
    
    public string FullName { get; set; } = null!;
    
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
