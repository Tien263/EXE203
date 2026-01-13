using Exe_Demo.Data;
using Exe_Demo.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Exe_Demo.Helpers
{
    /// <summary>
    /// Helper class để tạo tài khoản Staff
    /// </summary>
    public class StaffAccountHelper
    {
        private readonly ApplicationDbContext _context;

        public StaffAccountHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tạo tài khoản Staff mới
        /// </summary>
        public async Task<(bool Success, string Message, User? User)> CreateStaffAccountAsync(
            string email,
            string password,
            string fullName,
            string phoneNumber,
            string position,
            string department,
            decimal salary,
            string role = "Staff")
        {
            try
            {
                // Kiểm tra email đã tồn tại
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    return (false, "Email đã tồn tại trong hệ thống", null);
                }

                // Tạo Employee
                var employeeCode = await GenerateEmployeeCodeAsync();
                var employee = new Employee
                {
                    EmployeeCode = employeeCode,
                    FullName = fullName,
                    PhoneNumber = phoneNumber,
                    Email = email,
                    Position = position,
                    Department = department,
                    Salary = salary,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Tạo User
                var user = new User
                {
                    Email = email,
                    PasswordHash = HashPassword(password),
                    FullName = fullName,
                    PhoneNumber = phoneNumber,
                    Role = role,
                    EmployeeId = employee.EmployeeId,
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return (true, $"Tạo tài khoản {role} thành công! Mã nhân viên: {employeeCode}", user);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Tạo mã nhân viên tự động
        /// </summary>
        private async Task<string> GenerateEmployeeCodeAsync()
        {
            var lastEmployee = await _context.Employees
                .OrderByDescending(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastEmployee != null && !string.IsNullOrEmpty(lastEmployee.EmployeeCode))
            {
                var numberPart = lastEmployee.EmployeeCode.Replace("NV", "");
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"NV{nextNumber:D3}";
        }

        /// <summary>
        /// Hash password bằng SHA256
        /// </summary>
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Tạo tài khoản Staff mẫu cho testing
        /// </summary>
        public async Task<string> CreateSampleStaffAccountsAsync()
        {
            var results = new List<string>();

            // Tạo Staff account
            var staffResult = await CreateStaffAccountAsync(
                email: "staff@mocvistore.com",
                password: "Staff@123",
                fullName: "Nguyễn Văn A",
                phoneNumber: "0901234567",
                position: "Nhân viên bán hàng",
                department: "Bán hàng",
                salary: 8000000,
                role: "Staff"
            );
            results.Add(staffResult.Message);

            // Tạo Admin account
            var adminResult = await CreateStaffAccountAsync(
                email: "admin@mocvistore.com",
                password: "Admin@123",
                fullName: "Quản Trị Viên",
                phoneNumber: "0909999999",
                position: "Quản lý",
                department: "Quản lý",
                salary: 15000000,
                role: "Admin"
            );
            results.Add(adminResult.Message);

            return string.Join("\n", results);
        }
    }
}
