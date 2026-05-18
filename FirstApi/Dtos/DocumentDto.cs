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
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int OwnerUserId { get; set; }
    }
}
