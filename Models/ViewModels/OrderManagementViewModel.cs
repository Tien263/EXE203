using System;
using System.Collections.Generic;

namespace Exe_Demo.Models.ViewModels
{
    public class OrderManagementViewModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
        
        // Filters
        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }
        public string? PaymentStatusFilter { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class OrderDetailViewModel
    {
        public Order Order { get; set; } = null!;
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public Customer? Customer { get; set; }
        public Employee? Employee { get; set; }
    }

    public class UpdateOrderStatusViewModel
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string? PaymentStatus { get; set; }
        public string? Note { get; set; }
    }
}
