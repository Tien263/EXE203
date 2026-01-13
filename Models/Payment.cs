using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? TransactionCode { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual Order Order { get; set; } = null!;
}
