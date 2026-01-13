using Exe_Demo.Models;
using Exe_Demo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Exe_Demo.Services
{
    /// <summary>
    /// Cart Service - SOLID: Single Responsibility Principle
    /// Handles all shopping cart business logic with caching
    /// </summary>
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CartService> _logger;
        private const string CACHE_KEY_PREFIX = "Cart_";

        public CartService(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<CartService> logger)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<IEnumerable<Cart>> GetCartItemsAsync(int? customerId, string? sessionId)
        {
            try
            {
                var cacheKey = $"{CACHE_KEY_PREFIX}{customerId}_{sessionId}";
                
                return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    IQueryable<Cart> query = _unitOfWork.Carts.QueryNoTracking()
                        .Include(c => c.Product)
                        .ThenInclude(p => p.Category);

                    if (customerId.HasValue)
                    {
                        query = query.Where(c => c.CustomerId == customerId.Value);
                    }
                    else if (!string.IsNullOrEmpty(sessionId))
                    {
                        query = query.Where(c => c.SessionId == sessionId);
                    }

                    return await query.ToListAsync();
                }, TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items");
                throw;
            }
        }

        public async Task<Cart?> GetCartItemByIdAsync(int cartId)
        {
            try
            {
                return await _unitOfWork.Carts.GetByIdAsync(cartId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart item: {CartId}", cartId);
                throw;
            }
        }

        public async Task<bool> AddToCartAsync(int productId, int quantity, int? customerId, string? sessionId)
        {
            try
            {
                // Check if product exists and has stock
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null || product.StockQuantity < quantity)
                {
                    return false;
                }

                // Check if item already in cart
                var existingCart = await _unitOfWork.Carts.QueryNoTracking()
                    .FirstOrDefaultAsync(c => 
                        c.ProductId == productId &&
                        (customerId.HasValue ? c.CustomerId == customerId : c.SessionId == sessionId));

                if (existingCart != null)
                {
                    // Update quantity
                    existingCart.Quantity += quantity;
                    _unitOfWork.Carts.Update(existingCart);
                }
                else
                {
                    // Add new cart item
                    var cart = new Cart
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        CustomerId = customerId,
                        SessionId = sessionId,
                        CreatedDate = DateTime.Now
                    };
                    await _unitOfWork.Carts.AddAsync(cart);
                }

                await _unitOfWork.SaveChangesAsync();
                
                // Invalidate cache
                await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{customerId}_{sessionId}");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return false;
            }
        }

        public async Task<bool> UpdateQuantityAsync(int cartId, int quantity)
        {
            try
            {
                var cart = await _unitOfWork.Carts.GetByIdAsync(cartId);
                if (cart == null)
                {
                    return false;
                }

                // Check stock
                var product = await _unitOfWork.Products.GetByIdAsync(cart.ProductId);
                if (product == null || product.StockQuantity < quantity)
                {
                    return false;
                }

                cart.Quantity = quantity;
                _unitOfWork.Carts.Update(cart);
                await _unitOfWork.SaveChangesAsync();
                
                // Invalidate cache
                await _cacheService.RemoveByPrefixAsync(CACHE_KEY_PREFIX);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart quantity");
                return false;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int cartId)
        {
            try
            {
                var cart = await _unitOfWork.Carts.GetByIdAsync(cartId);
                if (cart == null)
                {
                    return false;
                }

                _unitOfWork.Carts.Remove(cart);
                await _unitOfWork.SaveChangesAsync();
                
                // Invalidate cache
                await _cacheService.RemoveByPrefixAsync(CACHE_KEY_PREFIX);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from cart");
                return false;
            }
        }

        public async Task<decimal> GetCartTotalAsync(int? customerId, string? sessionId)
        {
            try
            {
                var cartItems = await GetCartItemsAsync(customerId, sessionId);
                return cartItems.Sum(c => c.Product.Price * c.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating cart total");
                return 0;
            }
        }

        public async Task<int> GetCartItemCountAsync(int? customerId, string? sessionId)
        {
            try
            {
                var cartItems = await GetCartItemsAsync(customerId, sessionId);
                return cartItems.Sum(c => c.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart item count");
                return 0;
            }
        }

        public async Task ClearCartAsync(int? customerId, string? sessionId)
        {
            try
            {
                var cartItems = await _unitOfWork.Carts.Query()
                    .Where(c => customerId.HasValue 
                        ? c.CustomerId == customerId 
                        : c.SessionId == sessionId)
                    .ToListAsync();

                _unitOfWork.Carts.RemoveRange(cartItems);
                await _unitOfWork.SaveChangesAsync();
                
                // Invalidate cache
                await _cacheService.RemoveAsync($"{CACHE_KEY_PREFIX}{customerId}_{sessionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                throw;
            }
        }

        public async Task<bool> MergeCartAsync(string sessionId, int customerId)
        {
            try
            {
                // Get session cart items
                var sessionCarts = await _unitOfWork.Carts.Query()
                    .Where(c => c.SessionId == sessionId)
                    .ToListAsync();

                foreach (var sessionCart in sessionCarts)
                {
                    // Check if customer already has this product
                    var existingCart = await _unitOfWork.Carts.QueryNoTracking()
                        .FirstOrDefaultAsync(c => 
                            c.CustomerId == customerId && 
                            c.ProductId == sessionCart.ProductId);

                    if (existingCart != null)
                    {
                        // Merge quantities
                        existingCart.Quantity += sessionCart.Quantity;
                        _unitOfWork.Carts.Update(existingCart);
                    }
                    else
                    {
                        // Transfer to customer
                        sessionCart.CustomerId = customerId;
                        sessionCart.SessionId = null;
                        _unitOfWork.Carts.Update(sessionCart);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                
                // Invalidate cache
                await _cacheService.RemoveByPrefixAsync(CACHE_KEY_PREFIX);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error merging cart");
                return false;
            }
        }
    }
}
