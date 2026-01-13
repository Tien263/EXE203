using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exe_Demo.Data;

namespace Exe_Demo.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BlogController> _logger;

        public BlogController(ApplicationDbContext context, ILogger<BlogController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs
                .Where(b => b.IsPublished == true)
                .OrderByDescending(b => b.PublishedDate)
                .ToListAsync();

            return View(blogs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var blog = await _context.Blogs
                .FirstOrDefaultAsync(b => b.BlogId == id && b.IsPublished == true);

            if (blog == null)
            {
                return NotFound();
            }

            // Tăng view count
            blog.ViewCount = (blog.ViewCount ?? 0) + 1;
            await _context.SaveChangesAsync();

            return View(blog);
        }
    }
}
