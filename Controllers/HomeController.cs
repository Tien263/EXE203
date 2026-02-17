using System.Diagnostics;
using Exe_Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Exe_Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Services.IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, Services.IProductService productService)
        {
            _logger = logger;
            _configuration = configuration;
            _productService = productService;
        }
        
        [HttpGet]
        public IActionResult GetAIConfig()
        {
            var aiUrl = _configuration["AI:ApiUrl"] ?? "http://localhost:8000";
            return Json(new { apiUrl = aiUrl });
        }

        public async Task<IActionResult> Index()
        {
            try 
            {
                var featuredProducts = await _productService.GetFeaturedProductsAsync(8);
                var newProducts = await _productService.GetNewProductsAsync(8);

                var viewModel = new Models.ViewModels.HomeViewModel
                {
                    FeaturedProducts = featuredProducts,
                    NewProducts = newProducts
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page data");
                return View(new Models.ViewModels.HomeViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
