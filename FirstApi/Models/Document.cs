namespace FirstApi.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string EncryptedContent { get; set; } = string.Empty;
        public string ContentHash { get; set; } = string.Empty;
        public int OwnerUserId { get; set; }
        public User? OwnerUser { get; set; }
        public DateTime CreadtedAt { get; set; } = DateTime.UtcNow;

    }
}
