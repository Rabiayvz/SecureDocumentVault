using FirstApi.Dtos;

namespace FirstApi.Business.Services
{
    public interface IDocumentService
    {
        Guid CreateDocument(CreateDocumentDto dto);
        bool? VerifyDocumentContent(Guid documentId);
        List<DocumentResponseDto> GetDocuments();
        DocumentDetailResponseDto? GetDocumentById(Guid documentId);
    }
}