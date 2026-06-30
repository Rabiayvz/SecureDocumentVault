using System.ComponentModel.DataAnnotations;

namespace FirstApi.Dtos
{
    public class CreateDocumentDto
    {
        [Required]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Content cannot be empty.")]
        public string Content { get; set; } = string.Empty;
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
