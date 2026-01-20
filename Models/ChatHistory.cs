using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exe_Demo.Models
{
    public class ChatHistory
    {
        [Key]
        public int ChatHistoryId { get; set; }

        public int? CustomerId { get; set; }
        
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        public string? SessionId { get; set; } // For guest users

        [Required]
        public string Role { get; set; } = string.Empty; // "user" or "assistant"

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        // For order-related messages
        public bool IsOrderRelated { get; set; } = false;
        
        public string? OrderData { get; set; } // JSON data for order info
    }
}
