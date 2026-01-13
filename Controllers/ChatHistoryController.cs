using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Exe_Demo.Data;
using Exe_Demo.Models;

namespace Exe_Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChatHistoryController> _logger;

        public ChatHistoryController(ApplicationDbContext context, ILogger<ChatHistoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveChatMessage([FromBody] ChatMessageRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int? customerId = null;

                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var user = await _context.Users.FindAsync(userId);
                    customerId = user?.CustomerId;
                }

                var chatHistory = new ChatHistory
                {
                    CustomerId = customerId,
                    SessionId = request.SessionId,
                    Role = request.Role,
                    Message = request.Message,
                    CreatedDate = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IsOrderRelated = request.IsOrderRelated,
                    OrderData = request.OrderData
                };

                _context.ChatHistories.Add(chatHistory);
                await _context.SaveChangesAsync();

                return Json(new { success = true, chatHistoryId = chatHistory.ChatHistoryId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving chat history");
                return Json(new { success = false, message = "Lỗi khi lưu lịch sử chat" });
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerChatHistory(int customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var query = _context.ChatHistories
                    .Where(c => c.CustomerId == customerId)
                    .OrderByDescending(c => c.CreatedDate);

                var total = await query.CountAsync();
                var chats = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new
                    {
                        c.ChatHistoryId,
                        c.Role,
                        c.Message,
                        c.CreatedDate,
                        c.IsOrderRelated,
                        c.OrderData
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    total = total,
                    page = page,
                    pageSize = pageSize,
                    chats = chats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat history");
                return Json(new { success = false, message = "Lỗi khi lấy lịch sử chat" });
            }
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetSessionChatHistory(string sessionId)
        {
            try
            {
                var chats = await _context.ChatHistories
                    .Where(c => c.SessionId == sessionId)
                    .OrderBy(c => c.CreatedDate)
                    .Select(c => new
                    {
                        c.ChatHistoryId,
                        c.Role,
                        c.Message,
                        c.CreatedDate,
                        c.IsOrderRelated
                    })
                    .ToListAsync();

                return Json(new { success = true, chats = chats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting session chat history");
                return Json(new { success = false, message = "Lỗi khi lấy lịch sử chat" });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllChatHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            try
            {
                var query = _context.ChatHistories
                    .Include(c => c.Customer)
                    .OrderByDescending(c => c.CreatedDate);

                var total = await query.CountAsync();
                var chats = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new
                    {
                        c.ChatHistoryId,
                        c.CustomerId,
                        CustomerName = c.Customer != null ? c.Customer.FullName : "Khách",
                        c.SessionId,
                        c.Role,
                        c.Message,
                        c.CreatedDate,
                        c.IsOrderRelated,
                        c.IpAddress
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    total = total,
                    page = page,
                    pageSize = pageSize,
                    chats = chats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all chat history");
                return Json(new { success = false, message = "Lỗi khi lấy lịch sử chat" });
            }
        }
    }

    public class ChatMessageRequest
    {
        public string SessionId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "user" or "assistant"
        public string Message { get; set; } = string.Empty;
        public bool IsOrderRelated { get; set; } = false;
        public string? OrderData { get; set; }
    }
}
