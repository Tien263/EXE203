using System;
using System.Collections.Generic;

namespace Exe_Demo.Models.ViewModels
{
    public class SalesReportViewModel
    {
        // Bộ lọc
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ReportType { get; set; } = "daily"; // daily, weekly, monthly, yearly
        
        // Tổng quan
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public decimal AverageOrderValue { get; set; }
        
        // Biểu đồ doanh thu theo thời gian
        public List<RevenueByDateViewModel> RevenueByDate { get; set; } = new List<RevenueByDateViewModel>();
        
        // Top sản phẩm bán chạy
        public List<TopProductViewModel> TopProducts { get; set; } = new List<TopProductViewModel>();
        
        // Doanh thu theo danh mục
        public List<RevenueByCategoryViewModel> RevenueByCategory { get; set; } = new List<RevenueByCategoryViewModel>();
        
        // Doanh thu theo phương thức thanh toán
        public List<RevenueByPaymentMethodViewModel> RevenueByPaymentMethod { get; set; } = new List<RevenueByPaymentMethodViewModel>();
    }

    public class RevenueByDateViewModel
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class TopProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; }
    }

    public class RevenueByCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class RevenueByPaymentMethodViewModel
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int OrderCount { get; set; }
        public decimal Percentage { get; set; }
    }
}
