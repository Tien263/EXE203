using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Exe_Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIChatController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIChatController> _logger;

        public AIChatController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AIChatController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] object request)
        {
            try
            {
                var aiUrl = _configuration["AI:ApiUrl"] ?? "http://localhost:8000";
                var client = _httpClientFactory.CreateClient();

                // Forward the request to AI service
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync($"{aiUrl}/api/chat", jsonContent);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Return raw JSON from AI
                    return Content(content, "application/json");
                }
                else
                {
                    _logger.LogError($"AI Service Error: {response.StatusCode} - {content}");
                    return StatusCode((int)response.StatusCode, new { message = "Lỗi từ AI Service", details = content });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying chat request");
                return StatusCode(500, new { message = "Lỗi kết nối đến AI Service" });
            }
        }
        
        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
             try
             {
                 var aiUrl = _configuration["AI:ApiUrl"] ?? "http://localhost:8000";
                 var client = _httpClientFactory.CreateClient();
                 
                 var response = await client.GetAsync($"{aiUrl}/api/health");
                 var content = await response.Content.ReadAsStringAsync();
                 
                 if (response.IsSuccessStatusCode)
                 {
                     return Content(content, "application/json");
                 }
                 else 
                 {
                     return StatusCode((int)response.StatusCode, content);
                 }
             }
             catch
             {
                 return StatusCode(503, new { status = "unhealthy", message = "Cannot connect to AI Service" });
             }
        }
    }
}
