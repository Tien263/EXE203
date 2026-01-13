using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class Expense
{
    public int ExpenseId { get; set; }

    public string ExpenseCode { get; set; } = null!;

    public string? ExpenseType { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly ExpenseDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Employee? Employee { get; set; }
}
