using OfficeOpenXml;
using OfficeOpenXml.Style;
using Exe_Demo.Data;
using Exe_Demo.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Exe_Demo.Services
{
    public class ExcelOrderService
    {
        private readonly ApplicationDbContext _context;

        public ExcelOrderService(ApplicationDbContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<byte[]> ExportOrdersToExcel(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate)
                .OrderBy(o => o.CreatedDate)
                .ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Đơn Hàng");

                // Header
                worksheet.Cells[1, 1].Value = "Mã Đơn";
                worksheet.Cells[1, 2].Value = "Khách Hàng";
                worksheet.Cells[1, 3].Value = "SĐT";
                worksheet.Cells[1, 4].Value = "Địa Chỉ";
                worksheet.Cells[1, 5].Value = "Tổng Tiền";
                worksheet.Cells[1, 6].Value = "Ngày Đặt";
                worksheet.Cells[1, 7].Value = "Đã Thanh Toán";
                worksheet.Cells[1, 8].Value = "Chưa Thanh Toán";
                worksheet.Cells[1, 9].Value = "Chờ xử lý";
                worksheet.Cells[1, 10].Value = "Đang xử lý";
                worksheet.Cells[1, 11].Value = "Đang giao";
                worksheet.Cells[1, 12].Value = "Đã hoàn thành";
                worksheet.Cells[1, 13].Value = "Đã hủy";

                // Style header
                using (var range = worksheet.Cells[1, 1, 1, 13])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 106, 76));
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Data
                int row = 2;
                foreach (var order in orders)
                {
                    worksheet.Cells[row, 1].Value = order.OrderCode;
                    worksheet.Cells[row, 2].Value = order.CustomerName;
                    worksheet.Cells[row, 3].Value = order.CustomerPhone;
                    worksheet.Cells[row, 4].Value = order.ShippingAddress;
                    worksheet.Cells[row, 5].Value = order.FinalAmount;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,##0 ₫";
                    worksheet.Cells[row, 6].Value = order.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? "";

                    // Cột Đã Thanh Toán - Checkbox
                    var paidCell = worksheet.Cells[row, 7];
                    paidCell.Value = order.PaymentStatus == "Đã thanh toán" ? "☑" : "☐";
                    paidCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    paidCell.Style.Font.Name = "Segoe UI Symbol";
                    paidCell.Style.Font.Size = 16;

                    // Cột Chưa Thanh Toán - Checkbox
                    var unpaidCell = worksheet.Cells[row, 8];
                    unpaidCell.Value = order.PaymentStatus != "Đã thanh toán" ? "☑" : "☐";
                    unpaidCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    unpaidCell.Style.Font.Name = "Segoe UI Symbol";
                    unpaidCell.Style.Font.Size = 16;

                    // Tick trạng thái hiện tại - Checkbox
                    int statusCol = 9;
                    switch (order.OrderStatus)
                    {
                        case "Chờ xử lý":
                            worksheet.Cells[row, statusCol].Value = "☑";
                            break;
                        case "Đang xử lý":
                            worksheet.Cells[row, statusCol + 1].Value = "☑";
                            break;
                        case "Đang giao":
                            worksheet.Cells[row, statusCol + 2].Value = "☑";
                            break;
                        case "Đã hoàn thành":
                            worksheet.Cells[row, statusCol + 3].Value = "☑";
                            break;
                        case "Đã hủy":
                            worksheet.Cells[row, statusCol + 4].Value = "☑";
                            break;
                    }

                    // Style status columns with checkbox symbols
                    for (int col = 9; col <= 13; col++)
                    {
                        var cell = worksheet.Cells[row, col];
                        if (string.IsNullOrEmpty(cell.Value?.ToString()))
                        {
                            cell.Value = "☐";
                        }
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.Font.Name = "Segoe UI Symbol";
                        cell.Style.Font.Size = 16;
                    }

                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Add borders
                using (var range = worksheet.Cells[1, 1, row - 1, 13])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                // Hướng dẫn
                worksheet.Cells[row + 2, 1].Value = "HƯỚNG DẪN:";
                worksheet.Cells[row + 2, 1].Style.Font.Bold = true;
                worksheet.Cells[row + 3, 1].Value = "1. Thay đổi ☐ thành ☑ để tick checkbox (copy/paste ký tự)";
                worksheet.Cells[row + 4, 1].Value = "2. Thanh toán: Tick 'Đã Thanh Toán' HOẶC 'Chưa Thanh Toán' (chỉ chọn 1)";
                worksheet.Cells[row + 5, 1].Value = "3. Trạng thái đơn: Chỉ tick 1 trạng thái cho mỗi đơn";
                worksheet.Cells[row + 6, 1].Value = "4. Lưu file và import lại vào hệ thống";
                worksheet.Cells[row + 7, 1].Value = "5. KHÔNG XÓA hoặc thay đổi cột 'Mã Đơn'";

                return package.GetAsByteArray();
            }
        }

        public async Task<(int success, int failed, List<string> errors)> ImportOrdersFromExcel(Stream fileStream)
        {
            int successCount = 0;
            int failedCount = 0;
            var errors = new List<string>();

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var orderCode = worksheet.Cells[row, 1].Value?.ToString();
                        if (string.IsNullOrEmpty(orderCode)) continue;

                        var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
                        if (order == null)
                        {
                            errors.Add($"Dòng {row}: Không tìm thấy đơn hàng {orderCode}");
                            failedCount++;
                            continue;
                        }

                        // Lưu trạng thái cũ
                        var oldStatus = order.OrderStatus;

                        // Kiểm tra trạng thái thanh toán
                        var paidChecked = worksheet.Cells[row, 7].Value?.ToString();
                        var unpaidChecked = worksheet.Cells[row, 8].Value?.ToString();
                        
                        if (paidChecked == "☑" || paidChecked == "✓" || paidChecked?.ToLower() == "x")
                        {
                            order.PaymentStatus = "Đã thanh toán";
                        }
                        else if (unpaidChecked == "☑" || unpaidChecked == "✓" || unpaidChecked?.ToLower() == "x")
                        {
                            order.PaymentStatus = "Chưa thanh toán";
                        }

                        // Kiểm tra trạng thái đơn hàng được tick
                        string? newStatus = null;
                        if (worksheet.Cells[row, 9].Value?.ToString() == "☑") newStatus = "Chờ xử lý";
                        else if (worksheet.Cells[row, 10].Value?.ToString() == "☑") newStatus = "Đang xử lý";
                        else if (worksheet.Cells[row, 11].Value?.ToString() == "☑") newStatus = "Đang giao";
                        else if (worksheet.Cells[row, 12].Value?.ToString() == "☑") newStatus = "Đã hoàn thành";
                        else if (worksheet.Cells[row, 13].Value?.ToString() == "☑") newStatus = "Đã hủy";

                        if (string.IsNullOrEmpty(newStatus))
                        {
                            errors.Add($"Dòng {row}: Đơn {orderCode} không có trạng thái nào được tick");
                            failedCount++;
                            continue;
                        }

                        // Cập nhật trạng thái
                        order.OrderStatus = newStatus;
                        order.UpdatedDate = DateTime.Now;

                        // Nếu hoàn thành, cộng điểm
                        if (newStatus == "Đã hoàn thành" && oldStatus != "Đã hoàn thành" && order.CustomerId.HasValue)
                        {
                            var customer = await _context.Customers.FindAsync(order.CustomerId.Value);
                            if (customer != null)
                            {
                                int pointsToAdd = (int)(order.FinalAmount / 10000);
                                customer.LoyaltyPoints = (customer.LoyaltyPoints ?? 0) + pointsToAdd;
                            }
                            order.CompletedDate = DateTime.Now;
                        }

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Dòng {row}: Lỗi - {ex.Message}");
                        failedCount++;
                    }
                }

                await _context.SaveChangesAsync();
            }

            return (successCount, failedCount, errors);
        }
    }
}
