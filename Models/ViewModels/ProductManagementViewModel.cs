using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exe_Demo.Models.ViewModels
{
    public class ProductManagementViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
        
        // Filters
        public string? SearchTerm { get; set; }
        public int? CategoryFilter { get; set; }
        public string? StockFilter { get; set; } // "all", "low", "out"
    }

    public class ProductFormViewModel
    {
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Mã sản phẩm là bắt buộc")]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public int CategoryId { get; set; }
        
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? ShortDescription { get; set; }
        
        [Required(ErrorMessage = "Giá bán là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }
        
        public decimal? OriginalPrice { get; set; }
        
        public decimal? CostPrice { get; set; }
        
        [Range(0, 100)]
        public int? DiscountPercent { get; set; }
        
        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc")]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        
        public int? MinStockLevel { get; set; }
        
        [StringLength(50)]
        public string? Unit { get; set; }
        
        [StringLength(50)]
        public string? Weight { get; set; }
        
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
