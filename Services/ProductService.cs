using Exe_Demo.Models;
using Exe_Demo.Models.ViewModels;
using Exe_Demo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Exe_Demo.Services
{
    /// <summary>
    /// Product Service - SOLID: Single Responsibility Principle
    /// Handles all product-related business logic with caching
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ProductService> _logger;
        private const string CACHE_KEY_PREFIX = "Product_";
        private const string CACHE_KEY_LIST = "ProductList_";
        private const string CACHE_KEY_FEATURED = "FeaturedProducts";
        private const string CACHE_KEY_NEW = "NewProducts";

        public ProductService(
            IUnitOfWork unitOfWork, 
            ICacheService cacheService, 
            ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<ProductListViewModel> GetProductsAsync(
            int? categoryId, 
            string? search, 
            string? sortBy, 
            int pageNumber = 1, 
            int pageSize = 12)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_LIST}{categoryId}_{search}_{sortBy}_{pageNumber}_{pageSize}";
                
                return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var query = _unitOfWork.Products.QueryNoTracking()
                        .Include(p => p.Category)
                        .Where(p => p.IsActive == true);

                    // Filter by category
                    if (categoryId.HasValue && categoryId.Value > 0)
                    {
                        query = query.Where(p => p.CategoryId == categoryId.Value);
                    }

                    // Search
                    if (!string.IsNullOrEmpty(search))
                    {
                        query = query.Where(p => 
                            p.ProductName.Contains(search) || 
                            (p.Description != null && p.Description.Contains(search)));
                    }

                    // Sort
                    query = sortBy switch
                    {
                        "price_asc" => query.OrderBy(p => p.Price),
                        "price_desc" => query.OrderByDescending(p => p.Price),
                        "name" => query.OrderBy(p => p.ProductName),
                        "newest" => query.OrderByDescending(p => p.CreatedDate),
                        _ => query.OrderBy(p => p.ProductId)
                    };

                    var totalCount = await query.CountAsync();
                    var products = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

                    var categories = await _unitOfWork.Categories.QueryNoTracking()
                        .Where(c => c.IsActive == true)
                        .OrderBy(c => c.DisplayOrder)
                        .ToListAsync();

                    return new ProductListViewModel
                    {
                        Products = products,
                        Categories = categories,
                        CurrentCategory = categoryId,
                        CurrentSearch = search,
                        CurrentSort = sortBy,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                    };
                }, TimeSpan.FromMinutes(15));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products list");
                throw;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}{id}";
                
                return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    return await _unitOfWork.Products.GetByIdAsync(id);
                }, TimeSpan.FromMinutes(30));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product by id: {ProductId}", id);
                throw;
            }
        }

        public async Task<Product?> GetProductDetailsAsync(int id)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}Details_{id}";
                
                return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    return await _unitOfWork.Products.QueryNoTracking()
                        .Include(p => p.Category)
                        .Include(p => p.Reviews)
                        .FirstOrDefaultAsync(p => p.ProductId == id);
                }, TimeSpan.FromMinutes(20));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product details: {ProductId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int categoryId, int count = 4)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}Related_{productId}_{categoryId}_{count}";
                
                return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    return await _unitOfWork.Products.QueryNoTracking()
                        .Where(p => p.CategoryId == categoryId && p.ProductId != productId && p.IsActive == true)
                        .Take(count)
                        .ToListAsync();
                }, TimeSpan.FromMinutes(30));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting related products for: {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count = 8)
        {
            try
            {
                return await _cacheService.GetOrCreateAsync(CACHE_KEY_FEATURED, async () =>
                {
                    return await _unitOfWork.Products.QueryNoTracking()
                        .Where(p => p.IsFeatured == true && p.IsActive == true)
                        .OrderByDescending(p => p.CreatedDate)
                        .Take(count)
                        .ToListAsync();
                }, TimeSpan.FromMinutes(30));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured products");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetNewProductsAsync(int count = 8)
        {
            try
            {
                return await _cacheService.GetOrCreateAsync(CACHE_KEY_NEW, async () =>
                {
                    return await _unitOfWork.Products.QueryNoTracking()
                        .Where(p => p.IsNew == true && p.IsActive == true)
                        .OrderByDescending(p => p.CreatedDate)
                        .Take(count)
                        .ToListAsync();
                }, TimeSpan.FromMinutes(30));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting new products");
                throw;
            }
        }

        public async Task IncrementViewCountAsync(int productId)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product != null)
                {
                    product.ViewCount = (product.ViewCount ?? 0) + 1;
                    _unitOfWork.Products.Update(product);
                    await _unitOfWork.SaveChangesAsync();
                    
                    // Invalidate cache
                    await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{productId}");
                    await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}Details_{productId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing view count for: {ProductId}", productId);
                // Don't throw - view count is not critical
            }
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null || product.StockQuantity < quantity)
                {
                    return false;
                }

                product.StockQuantity -= quantity;
                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveChangesAsync();
                
                // Invalidate cache
                await _cacheService.RemoveByPrefixAsync(CACHE_KEY_PREFIX);
                await _cacheService.RemoveByPrefixAsync(CACHE_KEY_LIST);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for: {ProductId}", productId);
                return false;
            }
        }
    }
}
