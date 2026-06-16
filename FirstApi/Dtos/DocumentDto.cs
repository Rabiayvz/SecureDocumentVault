namespace FirstApi.Dtos
{
    public class CreateDocumentDto
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public Guid OwnerUserId { get; set; }
    }

    public class DocumentResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid OwnerUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DocumentDetailResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid OwnerUserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class VerifyDocumentDto
    {
        public string Content { get; set; } = string.Empty;
    }
}
