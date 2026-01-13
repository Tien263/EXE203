using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exe_Demo.Models.ViewModels
{
    public class DirectSaleViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<DirectSaleCartItem> CartItems { get; set; } = new List<DirectSaleCartItem>();
        
        public string? SearchTerm { get; set; }
        public int? CategoryFilter { get; set; }
        
        // Thông tin khách hàng
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        
        // Thông tin thanh toán
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; } = "Tiền mặt";
    }

    public class DirectSaleCartItem
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int? DiscountPercent { get; set; }
        public decimal TotalPrice { get; set; }
        public int StockQuantity { get; set; }
    }

    public class CreateDirectSaleOrderViewModel
    {
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string CustomerPhone { get; set; } = string.Empty;
        
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? CustomerEmail { get; set; }
        
        public int? CustomerId { get; set; }
        
        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        public string PaymentMethod { get; set; } = "Tiền mặt";
        
        public decimal DiscountAmount { get; set; }
        
        public string? Note { get; set; }
        
        public List<DirectSaleOrderItem> Items { get; set; } = new List<DirectSaleOrderItem>();
    }

    public class DirectSaleOrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
    }
}
