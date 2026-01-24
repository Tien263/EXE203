using Exe_Demo.Data;
using Exe_Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Exe_Demo.Controllers
{
    public class SystemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public SystemController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Status()
        {
            var model = new SystemStatusViewModel();

            // 1. Check Database Provider
            model.ProviderName = _context.Database.ProviderName;
            
            // 2. Check Connection String (Hide sensitive info)
            var connStr = _context.Database.GetConnectionString();
            model.ConnectionString = connStr != null 
                ? (connStr.Length > 50 ? connStr.Substring(0, 20) + "..." : connStr) 
                : "Null";

            // 3. Check Data Counts
            try 
            {
                model.CanConnect = _context.Database.CanConnect();
                if (model.CanConnect)
                {
                    model.UserCount = _context.Users.Count();
                    model.EmployeeCount = _context.Employees.Count();
                    model.ProductCount = _context.Products.Count();
                    
                    // Check specifically for Staff
                    var staff = _context.Users.FirstOrDefault(u => u.Email == "staff@mocvistore.com");
                    model.StaffStatus = staff != null 
                        ? $"Found (Active: {staff.IsActive}, ID: {staff.UserId})" 
                        : "NOT FOUND";
                        
                    var emp = _context.Employees.FirstOrDefault(e => e.Email == "staff@mocvistore.com");
                    model.StaffEmpStatus = emp != null
                        ? $"Found (ID: {emp.EmployeeId})"
                        : "NOT FOUND";
                }
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Seed()
        {
            try
            {
                DatabaseSeeder.SeedData(_context);
                TempData["Message"] = "Seeding completed successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Seeding Failed: {ex.Message} \n {ex.StackTrace}";
            }
            return RedirectToAction("Status");
        }
    }

    public class SystemStatusViewModel
    {
        public string? ProviderName { get; set; }
        public string? ConnectionString { get; set; }
        public bool CanConnect { get; set; }
        public int UserCount { get; set; }
        public int EmployeeCount { get; set; }
        public int ProductCount { get; set; }
        public string? StaffStatus { get; set; }
        public string? StaffEmpStatus { get; set; }
        public string? Error { get; set; }
    }
}
