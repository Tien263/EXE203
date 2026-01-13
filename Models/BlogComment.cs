using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class BlogComment
{
    public int CommentId { get; set; }

    public int BlogId { get; set; }

    public int? CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string? CustomerEmail { get; set; }

    public string Comment { get; set; } = null!;

    public bool? IsApproved { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual Customer? Customer { get; set; }
}
