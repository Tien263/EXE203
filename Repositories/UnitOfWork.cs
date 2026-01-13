using Exe_Demo.Data;
using Exe_Demo.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Exe_Demo.Repositories
{
    /// <summary>
    /// Unit of Work Implementation - Manages all repositories and transactions
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy initialization for repositories
        private IRepository<Product>? _products;
        private IRepository<Category>? _categories;
        private IRepository<Order>? _orders;
        private IRepository<OrderDetail>? _orderDetails;
        private IRepository<Customer>? _customers;
        private IRepository<Cart>? _carts;
        private IRepository<Review>? _reviews;
        private IRepository<Blog>? _blogs;
        private IRepository<BlogComment>? _blogComments;
        private IRepository<User>? _users;
        private IRepository<Employee>? _employees;
        private IRepository<Voucher>? _vouchers;
        private IRepository<Payment>? _payments;
        private IRepository<Supplier>? _suppliers;
        private IRepository<PurchaseOrder>? _purchaseOrders;
        private IRepository<PurchaseOrderDetail>? _purchaseOrderDetails;
        private IRepository<Shift>? _shifts;
        private IRepository<Expense>? _expenses;
        private IRepository<InventoryTransaction>? _inventoryTransactions;
        private IRepository<LoyaltyPointsHistory>? _loyaltyPointsHistories;
        private IRepository<ContactMessage>? _contactMessages;
        private IRepository<Setting>? _settings;
        private IRepository<OtpVerification>? _otpVerifications;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lazy-loaded repositories
        public IRepository<Product> Products => _products ??= new Repository<Product>(_context);
        public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
        public IRepository<Order> Orders => _orders ??= new Repository<Order>(_context);
        public IRepository<OrderDetail> OrderDetails => _orderDetails ??= new Repository<OrderDetail>(_context);
        public IRepository<Customer> Customers => _customers ??= new Repository<Customer>(_context);
        public IRepository<Cart> Carts => _carts ??= new Repository<Cart>(_context);
        public IRepository<Review> Reviews => _reviews ??= new Repository<Review>(_context);
        public IRepository<Blog> Blogs => _blogs ??= new Repository<Blog>(_context);
        public IRepository<BlogComment> BlogComments => _blogComments ??= new Repository<BlogComment>(_context);
        public IRepository<User> Users => _users ??= new Repository<User>(_context);
        public IRepository<Employee> Employees => _employees ??= new Repository<Employee>(_context);
        public IRepository<Voucher> Vouchers => _vouchers ??= new Repository<Voucher>(_context);
        public IRepository<Payment> Payments => _payments ??= new Repository<Payment>(_context);
        public IRepository<Supplier> Suppliers => _suppliers ??= new Repository<Supplier>(_context);
        public IRepository<PurchaseOrder> PurchaseOrders => _purchaseOrders ??= new Repository<PurchaseOrder>(_context);
        public IRepository<PurchaseOrderDetail> PurchaseOrderDetails => _purchaseOrderDetails ??= new Repository<PurchaseOrderDetail>(_context);
        public IRepository<Shift> Shifts => _shifts ??= new Repository<Shift>(_context);
        public IRepository<Expense> Expenses => _expenses ??= new Repository<Expense>(_context);
        public IRepository<InventoryTransaction> InventoryTransactions => _inventoryTransactions ??= new Repository<InventoryTransaction>(_context);
        public IRepository<LoyaltyPointsHistory> LoyaltyPointsHistories => _loyaltyPointsHistories ??= new Repository<LoyaltyPointsHistory>(_context);
        public IRepository<ContactMessage> ContactMessages => _contactMessages ??= new Repository<ContactMessage>(_context);
        public IRepository<Setting> Settings => _settings ??= new Repository<Setting>(_context);
        public IRepository<OtpVerification> OtpVerifications => _otpVerifications ??= new Repository<OtpVerification>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
