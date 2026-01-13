using System;
using System.Collections.Generic;

namespace Exe_Demo.Models.ViewModels
{
    public class StaffDashboardViewModel
    {
        // Thống kê tổng quan
        public decimal TodayRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public int TodayOrders { get; set; }
        public int MonthOrders { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockProducts { get; set; }
        
        // Đơn hàng gần đây
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        
        // Sản phẩm bán chạy
        public List<ProductSalesViewModel> TopSellingProducts { get; set; } = new List<ProductSalesViewModel>();
        
        // Thông tin nhân viên
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }

    public class ProductSalesViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
