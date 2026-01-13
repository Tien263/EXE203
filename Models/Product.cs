using System;
using System.Collections.Generic;

namespace Exe_Demo.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductCode { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? Description { get; set; }

    public string? ShortDescription { get; set; }

    public decimal Price { get; set; }

    public decimal? OriginalPrice { get; set; }

    public decimal? CostPrice { get; set; }

    public int? DiscountPercent { get; set; }

    public int? StockQuantity { get; set; }

    public int? MinStockLevel { get; set; }

    public string? Unit { get; set; }

    public string? Weight { get; set; }

    public string? ImageUrl { get; set; }

    public string? ImageGallery { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsFeatured { get; set; }

    public bool? IsNew { get; set; }

    public int? ViewCount { get; set; }

    public int? SoldCount { get; set; }

    public decimal? Rating { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
