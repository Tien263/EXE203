namespace Exe_Demo.Models.ViewModels
{
    public class ChatHistoryViewModel
    {
        public List<ChatSessionGroup> Sessions { get; set; } = new List<ChatSessionGroup>();
        public int TotalSessions { get; set; }
        public int TotalMessages { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class ChatSessionGroup
    {
        public string SessionId { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; } = "Khách vãng lai";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MessageCount { get; set; }
        public bool HasOrderRelated { get; set; }
        public List<ChatHistoryMessage> Messages { get; set; } = new List<ChatHistoryMessage>();
    }

    public class ChatHistoryMessage
    {
        public int ChatHistoryId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsOrderRelated { get; set; }
        public string? OrderData { get; set; }
    }
}
