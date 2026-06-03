namespace FirstApi.Dtos
{
    public class CreateDocumentDto
    {
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int OwnerUserId { get; set; }
    }

    public class DocumentResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OwnerUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DocumentDetailResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OwnerUserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
