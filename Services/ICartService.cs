using Exe_Demo.Models;

namespace Exe_Demo.Services
{
    /// <summary>
    /// Cart Service Interface - SOLID: Interface Segregation Principle
    /// </summary>
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetCartItemsAsync(int? customerId, string? sessionId);
        Task<Cart?> GetCartItemByIdAsync(int cartId);
        Task<bool> AddToCartAsync(int productId, int quantity, int? customerId, string? sessionId);
        Task<bool> UpdateQuantityAsync(int cartId, int quantity);
        Task<bool> RemoveFromCartAsync(int cartId);
        Task<decimal> GetCartTotalAsync(int? customerId, string? sessionId);
        Task<int> GetCartItemCountAsync(int? customerId, string? sessionId);
        Task ClearCartAsync(int? customerId, string? sessionId);
        Task<bool> MergeCartAsync(string sessionId, int customerId);
    }
}
