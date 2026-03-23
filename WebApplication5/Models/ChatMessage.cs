namespace WebApplication5.Models
{
    public class ChatMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPrivate { get; set; }
        public string ChannelKey { get; set; } = "__public__";
        public string? ToUser { get; set; }
    }
}
