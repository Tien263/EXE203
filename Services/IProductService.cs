using Exe_Demo.Models;
using Exe_Demo.Models.ViewModels;

namespace Exe_Demo.Services
{
    /// <summary>
    /// Product Service Interface - SOLID: Interface Segregation Principle
    /// </summary>
    public interface IProductService
    {
        Task<ProductListViewModel> GetProductsAsync(int? categoryId, string? search, string? sortBy, int pageNumber = 1, int pageSize = 12);
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product?> GetProductDetailsAsync(int id);
        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int categoryId, int count = 4);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count = 8);
        Task<IEnumerable<Product>> GetNewProductsAsync(int count = 8);
        Task IncrementViewCountAsync(int productId);
        Task<bool> UpdateStockAsync(int productId, int quantity);
    }
}
