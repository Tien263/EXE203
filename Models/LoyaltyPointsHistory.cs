using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class LoyaltyPointsHistory
{
    public int HistoryId { get; set; }

    public int CustomerId { get; set; }

    public int? OrderId { get; set; }

    public int Points { get; set; }

    public string? TransactionType { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Order? Order { get; set; }
}
