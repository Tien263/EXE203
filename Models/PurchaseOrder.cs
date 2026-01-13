using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class PurchaseOrder
{
    public int PurchaseOrderId { get; set; }

    public string PurchaseOrderCode { get; set; } = null!;

    public int SupplierId { get; set; }

    public int? EmployeeId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? PaidAmount { get; set; }

    public decimal? RemainingAmount { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

    public virtual Supplier Supplier { get; set; } = null!;
}
