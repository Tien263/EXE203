using Exe_Demo.Models;

namespace Exe_Demo.Repositories
{
    /// <summary>
    /// Unit of Work Pattern - SOLID: Single Responsibility Principle
    /// Manages transactions and coordinates repositories
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IRepository<Product> Products { get; }
        IRepository<Category> Categories { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderDetail> OrderDetails { get; }
        IRepository<Customer> Customers { get; }
        IRepository<Cart> Carts { get; }
        IRepository<Review> Reviews { get; }
        IRepository<Blog> Blogs { get; }
        IRepository<BlogComment> BlogComments { get; }
        IRepository<User> Users { get; }
        IRepository<Employee> Employees { get; }
        IRepository<Voucher> Vouchers { get; }
        IRepository<Payment> Payments { get; }
        IRepository<Supplier> Suppliers { get; }
        IRepository<PurchaseOrder> PurchaseOrders { get; }
        IRepository<PurchaseOrderDetail> PurchaseOrderDetails { get; }
        IRepository<Shift> Shifts { get; }
        IRepository<Expense> Expenses { get; }
        IRepository<InventoryTransaction> InventoryTransactions { get; }
        IRepository<LoyaltyPointsHistory> LoyaltyPointsHistories { get; }
        IRepository<ContactMessage> ContactMessages { get; }
        IRepository<Setting> Settings { get; }
        IRepository<OtpVerification> OtpVerifications { get; }

        // Transaction management
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
