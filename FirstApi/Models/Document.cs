namespace FirstApi.Models
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string EncryptedContent { get; set; } = string.Empty;
        public string ContentHash { get; set; } = string.Empty;
        public Guid OwnerUserId { get; set; }
        public User? OwnerUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
