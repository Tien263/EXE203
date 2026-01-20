using Microsoft.EntityFrameworkCore;

namespace Exe_Demo.Data
{
    /// <summary>
    /// Database Index Configuration for Performance Optimization
    /// Adds indexes to frequently queried columns
    /// </summary>
    public static class DatabaseIndexConfiguration
    {
        public static void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Product Indexes - Most frequently queried table
            modelBuilder.Entity<Models.Product>()
                .HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            modelBuilder.Entity<Models.Product>()
                .HasIndex(p => p.IsActive)
                .HasDatabaseName("IX_Products_IsActive");

            modelBuilder.Entity<Models.Product>()
                .HasIndex(p => p.IsFeatured)
                .HasDatabaseName("IX_Products_IsFeatured");

            modelBuilder.Entity<Models.Product>()
                .HasIndex(p => p.IsNew)
                .HasDatabaseName("IX_Products_IsNew");

            modelBuilder.Entity<Models.Product>()
                .HasIndex(p => p.Price)
                .HasDatabaseName("IX_Products_Price");

            modelBuilder.Entity<Models.Product>()
                .HasIndex(p => p.CreatedDate)
                .HasDatabaseName("IX_Products_CreatedDate");

            modelBuilder.Entity<Models.Product>()
                .HasIndex(p => new { p.CategoryId, p.IsActive })
                .HasDatabaseName("IX_Products_CategoryId_IsActive");

            // Order Indexes
            modelBuilder.Entity<Models.Order>()
                .HasIndex(o => o.CustomerId)
                .HasDatabaseName("IX_Orders_CustomerId");

            modelBuilder.Entity<Models.Order>()
                .HasIndex(o => o.OrderStatus)
                .HasDatabaseName("IX_Orders_OrderStatus");

            modelBuilder.Entity<Models.Order>()
                .HasIndex(o => o.CreatedDate)
                .HasDatabaseName("IX_Orders_CreatedDate");

            modelBuilder.Entity<Models.Order>()
                .HasIndex(o => new { o.CustomerId, o.OrderStatus })
                .HasDatabaseName("IX_Orders_CustomerId_OrderStatus");

            // OrderDetail Indexes
            modelBuilder.Entity<Models.OrderDetail>()
                .HasIndex(od => od.OrderId)
                .HasDatabaseName("IX_OrderDetails_OrderId");

            modelBuilder.Entity<Models.OrderDetail>()
                .HasIndex(od => od.ProductId)
                .HasDatabaseName("IX_OrderDetails_ProductId");

            // Customer Indexes
            modelBuilder.Entity<Models.Customer>()
                .HasIndex(c => c.Email)
                .HasDatabaseName("IX_Customers_Email");

            modelBuilder.Entity<Models.Customer>()
                .HasIndex(c => c.PhoneNumber)
                .HasDatabaseName("IX_Customers_PhoneNumber");

            modelBuilder.Entity<Models.Customer>()
                .HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Customers_IsActive");

            // Cart Indexes
            modelBuilder.Entity<Models.Cart>()
                .HasIndex(c => c.CustomerId)
                .HasDatabaseName("IX_Cart_CustomerId");

            modelBuilder.Entity<Models.Cart>()
                .HasIndex(c => c.SessionId)
                .HasDatabaseName("IX_Cart_SessionId");

            modelBuilder.Entity<Models.Cart>()
                .HasIndex(c => c.ProductId)
                .HasDatabaseName("IX_Cart_ProductId");

            // Category Indexes
            modelBuilder.Entity<Models.Category>()
                .HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Categories_IsActive");

            modelBuilder.Entity<Models.Category>()
                .HasIndex(c => c.DisplayOrder)
                .HasDatabaseName("IX_Categories_DisplayOrder");

            // Blog Indexes
            modelBuilder.Entity<Models.Blog>()
                .HasIndex(b => b.IsPublished)
                .HasDatabaseName("IX_Blogs_IsPublished");

            modelBuilder.Entity<Models.Blog>()
                .HasIndex(b => b.PublishedDate)
                .HasDatabaseName("IX_Blogs_PublishedDate");

            modelBuilder.Entity<Models.Blog>()
                .HasIndex(b => b.AuthorId)
                .HasDatabaseName("IX_Blogs_AuthorId");

            // Review Indexes
            modelBuilder.Entity<Models.Review>()
                .HasIndex(r => r.ProductId)
                .HasDatabaseName("IX_Reviews_ProductId");

            modelBuilder.Entity<Models.Review>()
                .HasIndex(r => r.IsApproved)
                .HasDatabaseName("IX_Reviews_IsApproved");

            // User Indexes
            modelBuilder.Entity<Models.User>()
                .HasIndex(u => u.IsActive)
                .HasDatabaseName("IX_Users_IsActive");

            modelBuilder.Entity<Models.User>()
                .HasIndex(u => u.Role)
                .HasDatabaseName("IX_Users_Role");

            // Employee Indexes
            modelBuilder.Entity<Models.Employee>()
                .HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Employees_IsActive");

            modelBuilder.Entity<Models.Employee>()
                .HasIndex(e => e.Department)
                .HasDatabaseName("IX_Employees_Department");

            // OTP Verification Indexes
            modelBuilder.Entity<Models.OtpVerification>()
                .HasIndex(o => o.Email)
                .HasDatabaseName("IX_OtpVerifications_Email");

            modelBuilder.Entity<Models.OtpVerification>()
                .HasIndex(o => o.ExpiresAt)
                .HasDatabaseName("IX_OtpVerifications_ExpiresAt");

            modelBuilder.Entity<Models.OtpVerification>()
                .HasIndex(o => new { o.Email, o.IsUsed })
                .HasDatabaseName("IX_OtpVerifications_Email_IsUsed");
        }
    }
}
