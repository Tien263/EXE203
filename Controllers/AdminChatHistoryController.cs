using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exe_Demo.Data;
using Exe_Demo.Models.ViewModels;

namespace Exe_Demo.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminChatHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminChatHistoryController> _logger;

        public AdminChatHistoryController(ApplicationDbContext context, ILogger<AdminChatHistoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: AdminChatHistory
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            try
            {
                // Get all sessions grouped
                var sessionsQuery = _context.ChatHistories
                    .Include(c => c.Customer)
                    .GroupBy(c => c.SessionId)
                    .Select(g => new
                    {
                        SessionId = g.Key,
                        CustomerId = g.FirstOrDefault()!.CustomerId,
                        CustomerName = g.FirstOrDefault()!.Customer != null 
                            ? g.FirstOrDefault()!.Customer.FullName 
                            : "Khách vãng lai",
                        StartTime = g.Min(c => c.CreatedDate),
                        EndTime = g.Max(c => c.CreatedDate),
                        MessageCount = g.Count(),
                        HasOrderRelated = g.Any(c => c.IsOrderRelated)
                    })
                    .OrderByDescending(s => s.StartTime);

                var totalSessions = await sessionsQuery.CountAsync();
                var totalPages = (int)Math.Ceiling(totalSessions / (double)pageSize);

                var sessions = await sessionsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModel = new ChatHistoryViewModel
                {
                    Sessions = sessions.Select(s => new ChatSessionGroup
                    {
                        SessionId = s.SessionId ?? "",
                        CustomerId = s.CustomerId,
                        CustomerName = s.CustomerName,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        MessageCount = s.MessageCount,
                        HasOrderRelated = s.HasOrderRelated
                    }).ToList(),
                    TotalSessions = totalSessions,
                    TotalMessages = await _context.ChatHistories.CountAsync(),
                    CurrentPage = page,
                    TotalPages = totalPages
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading chat history");
                return View(new ChatHistoryViewModel());
            }
        }

        // GET: AdminChatHistory/Details/sessionId
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var messages = await _context.ChatHistories
                    .Include(c => c.Customer)
                    .Where(c => c.SessionId == id)
                    .OrderBy(c => c.CreatedDate)
                    .Select(c => new ChatHistoryMessage
                    {
                        ChatHistoryId = c.ChatHistoryId,
                        Role = c.Role,
                        Message = c.Message,
                        CreatedDate = c.CreatedDate,
                        IsOrderRelated = c.IsOrderRelated,
                        OrderData = c.OrderData
                    })
                    .ToListAsync();

                if (!messages.Any())
                {
                    return NotFound();
                }

                var session = await _context.ChatHistories
                    .Include(c => c.Customer)
                    .Where(c => c.SessionId == id)
                    .GroupBy(c => c.SessionId)
                    .Select(g => new ChatSessionGroup
                    {
                        SessionId = g.Key ?? "",
                        CustomerId = g.FirstOrDefault()!.CustomerId,
                        CustomerName = g.FirstOrDefault()!.Customer != null 
                            ? g.FirstOrDefault()!.Customer.FullName 
                            : "Khách vãng lai",
                        StartTime = g.Min(c => c.CreatedDate),
                        EndTime = g.Max(c => c.CreatedDate),
                        MessageCount = g.Count(),
                        HasOrderRelated = g.Any(c => c.IsOrderRelated),
                        Messages = messages
                    })
                    .FirstOrDefaultAsync();

                return View(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading chat session details");
                return NotFound();
            }
        }

        // POST: AdminChatHistory/Delete/sessionId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var messages = await _context.ChatHistories
                    .Where(c => c.SessionId == id)
                    .ToListAsync();

                if (messages.Any())
                {
                    _context.ChatHistories.RemoveRange(messages);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã xóa lịch sử chat thành công!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting chat session");
                TempData["ErrorMessage"] = "Có lỗi khi xóa lịch sử chat!";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
