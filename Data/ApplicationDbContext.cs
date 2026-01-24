using System;
using System.Collections.Generic;
using Exe_Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace Exe_Demo.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public required virtual DbSet<Blog> Blogs { get; set; }

    public required virtual DbSet<BlogComment> BlogComments { get; set; }

    public required virtual DbSet<Cart> Carts { get; set; }

    public required virtual DbSet<Category> Categories { get; set; }

    public required virtual DbSet<ChatHistory> ChatHistories { get; set; }

    public required virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public required virtual DbSet<Customer> Customers { get; set; }

    public required virtual DbSet<Employee> Employees { get; set; }

    public required virtual DbSet<Expense> Expenses { get; set; }

    public required virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    public required virtual DbSet<LoyaltyPointsHistory> LoyaltyPointsHistories { get; set; }

    public required virtual DbSet<Order> Orders { get; set; }

    public required virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public required virtual DbSet<Payment> Payments { get; set; }

    public required virtual DbSet<Product> Products { get; set; }

    public required virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public required virtual DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    public required virtual DbSet<Review> Reviews { get; set; }

    public required virtual DbSet<Setting> Settings { get; set; }

    public required virtual DbSet<Shift> Shifts { get; set; }

    public required virtual DbSet<Supplier> Suppliers { get; set; }

    public required virtual DbSet<User> Users { get; set; }

    public required virtual DbSet<Voucher> Vouchers { get; set; }

    public required virtual DbSet<OtpVerification> OtpVerifications { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Connection string được cấu hình trong Program.cs từ appsettings.json
        // Không cần hardcode ở đây
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__54379E30D7FB4623");

            entity.HasIndex(e => e.Slug, "UQ__Blogs__BC7B5FB6F961F801").IsUnique();

            entity.Property(e => e.AuthorName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.IsPublished).HasDefaultValue(true);
            entity.Property(e => e.PublishedDate).HasColumnType("datetime");
            entity.Property(e => e.ShortDescription).HasMaxLength(500);
            entity.Property(e => e.Slug).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Blogs__AuthorId__2FCF1A8A");
        });

        modelBuilder.Entity<BlogComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__BlogComm__C3B4DFCAB7874396");

            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerEmail).HasMaxLength(100);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogComments)
                .HasForeignKey(d => d.BlogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BlogComme__BlogI__3493CFA7");

            entity.HasOne(d => d.Customer).WithMany(p => p.BlogComments)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__BlogComme__Custo__3587F3E0");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7B77262D876");

            entity.ToTable("Cart");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.SessionId).HasMaxLength(100);

            entity.HasOne(d => d.Customer).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Cart__CustomerId__2180FB33");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cart__ProductId__22751F6C");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BEB7A4BFE");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__ContactM__C87C0C9C21A4A6C8");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.IsReplied).HasDefaultValue(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Subject).HasMaxLength(255);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D8363B03CF");

            entity.HasIndex(e => e.CustomerCode, "UQ__Customer__06678521EFABD034").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerCode).HasMaxLength(50);
            entity.Property(e => e.CustomerType)
                .HasMaxLength(50)
                .HasDefaultValue("ThÆ°á»ng");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastPurchaseDate).HasColumnType("datetime");
            entity.Property(e => e.LoyaltyPoints).HasDefaultValue(0);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.TotalOrders).HasDefaultValue(0);
            entity.Property(e => e.TotalPurchased)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Ward).HasMaxLength(100);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11FE470046");

            entity.HasIndex(e => e.EmployeeCode, "UQ__Employee__1F64254817C2AECF").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BankAccount).HasMaxLength(50);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IdentityCard).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__Expenses__1445CFD3CEB81C45");

            entity.HasIndex(e => e.ExpenseCode, "UQ__Expenses__5064CAE114428009").IsUnique();

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ExpenseCode).HasMaxLength(50);
            entity.Property(e => e.ExpenseType).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ÄÃ£ chi");

            entity.HasOne(d => d.Employee).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Expenses__Employ__40058253");
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Inventor__55433A6BA758D326");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.ReferenceType).HasMaxLength(50);
            entity.Property(e => e.TransactionType).HasMaxLength(50);

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__5CD6CB2B");
        });

        modelBuilder.Entity<LoyaltyPointsHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__LoyaltyP__4D7B4ABDD68DD771");

            entity.ToTable("LoyaltyPointsHistory");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TransactionType).HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithMany(p => p.LoyaltyPointsHistories)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoyaltyPo__Custo__1332DBDC");

            entity.HasOne(d => d.Order).WithMany(p => p.LoyaltyPointsHistories)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__LoyaltyPo__Order__14270015");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFF01C29D3");

            entity.HasIndex(e => e.OrderCode, "UQ__Orders__999B52299F25A08F").IsUnique();

            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CompletedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerEmail).HasMaxLength(100);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoyaltyPointsEarned).HasDefaultValue(0);
            entity.Property(e => e.LoyaltyPointsUsed).HasDefaultValue(0);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.OrderCode).HasMaxLength(50);
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Chá» xÃ¡c nháº­n");
            entity.Property(e => e.OrderType)
                .HasMaxLength(50)
                .HasDefaultValue("Online");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("ChÆ°a thanh toÃ¡n");
            entity.Property(e => e.ShippingAddress).HasMaxLength(255);
            entity.Property(e => e.ShippingFee)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.VoucherCode).HasMaxLength(50);
            entity.Property(e => e.Ward).HasMaxLength(100);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Orders__Customer__7D439ABD");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Orders__Employee__7E37BEF6");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D36C71A16D84");

            entity.Property(e => e.DiscountPercent).HasDefaultValue(0);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__02084FDA");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Produ__02FC7413");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38BCA55591");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("ThÃ nh cÃ´ng");
            entity.Property(e => e.TransactionCode).HasMaxLength(100);

            entity.HasOne(d => d.Employee).WithMany(p => p.Payments)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Payments__Employ__08B54D69");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__OrderI__07C12930");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDEA842406");

            entity.HasIndex(e => e.ProductCode, "UQ__Products__2F4E024F2D65F9C7").IsUnique();

            entity.Property(e => e.CostPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountPercent).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsFeatured).HasDefaultValue(false);
            entity.Property(e => e.IsNew).HasDefaultValue(false);
            entity.Property(e => e.MinStockLevel).HasDefaultValue(10);
            entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductCode).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Rating)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.ShortDescription).HasMaxLength(500);
            entity.Property(e => e.SoldCount).HasDefaultValue(0);
            entity.Property(e => e.StockQuantity).HasDefaultValue(0);
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasDefaultValue("gÃ³i");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);
            entity.Property(e => e.Weight).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__47DBAE45");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderId).HasName("PK__Purchase__036BACA4BA65F84C");

            entity.HasIndex(e => e.PurchaseOrderCode, "UQ__Purchase__E282C5B440FB451B").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaidAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PurchaseOrderCode).HasMaxLength(50);
            entity.Property(e => e.ReceivedDate).HasColumnType("datetime");
            entity.Property(e => e.RemainingAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chá» duyá»‡t");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Suppl__5535A963");
        });

        modelBuilder.Entity<PurchaseOrderDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderDetailId).HasName("PK__Purchase__5026B6986489F6F8");

            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Produ__59063A47");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderDetails)
                .HasForeignKey(d => d.PurchaseOrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__Purch__5812160E");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE911E2C1F");

            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Reviews__Custome__29221CFB");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__Product__282DF8C2");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PK__Settings__54372B1DFC4958A2");

            entity.HasIndex(e => e.SettingKey, "UQ__Settings__01E719AD5D4C6367").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.SettingKey).HasMaxLength(100);
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("PK__Shifts__C0A83881BE5A36B7");

            entity.HasIndex(e => e.ShiftCode, "UQ__Shifts__9377D562DFAD97A7").IsUnique();

            entity.Property(e => e.ClosingCash).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OpeningCash)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ShiftCode).HasMaxLength(50);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Äang má»Ÿ");
            entity.Property(e => e.TotalOrders).HasDefaultValue(0);
            entity.Property(e => e.TotalSales)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Employee).WithMany(p => p.Shifts)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shifts__Employee__1CBC4616");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B4D1C018D4");

            entity.HasIndex(e => e.SupplierCode, "UQ__Supplier__44BE981B6C09723B").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BankAccount).HasMaxLength(50);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.ContactPerson).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.SupplierCode).HasMaxLength(50);
            entity.Property(e => e.SupplierName).HasMaxLength(200);
            entity.Property(e => e.TaxCode).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C2A3ED859");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534D9539C96").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Customer");

            entity.HasOne(d => d.Customer).WithMany(p => p.Users)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Users__CustomerI__71D1E811");

            entity.HasOne(d => d.Employee).WithMany(p => p.Users)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Users__EmployeeI__70DDC3D8");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__Vouchers__3AEE79213F792BF5");

            entity.HasIndex(e => e.VoucherCode, "UQ__Vouchers__7F0ABCA91B0360D6").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountType).HasMaxLength(20);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaxDiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MinOrderAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UsedCount).HasDefaultValue(0);
            entity.Property(e => e.ValidFrom).HasColumnType("datetime");
            entity.Property(e => e.ValidTo).HasColumnType("datetime");
            entity.Property(e => e.VoucherCode).HasMaxLength(50);
            entity.Property(e => e.VoucherName).HasMaxLength(200);
        });

        modelBuilder.Entity<OtpVerification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.OtpCode)
                .IsRequired()
                .HasMaxLength(6);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime");

            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false);
        });

        // Configure performance indexes
        DatabaseIndexConfiguration.ConfigureIndexes(modelBuilder);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
