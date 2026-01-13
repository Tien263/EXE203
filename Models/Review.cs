using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    public int? CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Product Product { get; set; } = null!;
}
