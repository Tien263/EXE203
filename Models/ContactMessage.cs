using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class ContactMessage
{
    public int MessageId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Subject { get; set; }

    public string Message { get; set; } = null!;

    public bool? IsRead { get; set; }

    public bool? IsReplied { get; set; }

    public DateTime? CreatedDate { get; set; }
}
