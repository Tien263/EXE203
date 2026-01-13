using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class Shift
{
    public int ShiftId { get; set; }

    public string ShiftCode { get; set; } = null!;

    public int EmployeeId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal? OpeningCash { get; set; }

    public decimal? ClosingCash { get; set; }

    public decimal? TotalSales { get; set; }

    public int? TotalOrders { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
