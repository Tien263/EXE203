using Microsoft.AspNetCore.Mvc;
using Exe_Demo.Services;
using Exe_Demo.Models;

namespace Exe_Demo.Controllers
{
    /// <summary>
    /// Product Controller - SOLID: Dependency Injection, uses ProductService
    /// </summary>
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: Product
        // [ResponseCache(CacheProfileName = "Default30")] // Removed to fix User Authentication display issue
        public async Task<IActionResult> Index(int? categoryId, string? search, string? sortBy, int pageNumber = 1)
        {
            try
            {
                var viewModel = await _productService.GetProductsAsync(
                    categoryId, 
                    search, 
                    sortBy, 
                    pageNumber, 
                    pageSize: 12);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products");
                return View("Error");
            }
        }

        // GET: Product/Details/5
        // [ResponseCache(Duration = 600, VaryByQueryKeys = new[] { "id" })] // Removed to fix User Authentication display issue
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetProductDetailsAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                // Increment view count (sequentially to avoid DbContext threading issues)
                await _productService.IncrementViewCountAsync(id);

                // Get related products
                var relatedProducts = await _productService.GetRelatedProductsAsync(
                    id, 
                    product.CategoryId, 
                    count: 4);

                ViewBag.RelatedProducts = relatedProducts;

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading product details: {ProductId}", id);
                return View("Error");
            }
        }

        // GET: Product/Category/1
        public IActionResult Category(int id)
        {
            return RedirectToAction(nameof(Index), new { categoryId = id });
        }
    }
}
