using FirstApi.Data;
using FirstApi.Dtos;
using FirstApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace FirstApi.Business.Services
{
    public class DocumentService
    {
        private readonly AppDbContext _context;

        public DocumentService(AppDbContext context)
        {
            _context = context;
        }

        //POST /documents
        //Kullanıcı → DTO → Service → Entity → Database
        public int CreateDocument(CreateDocumentDto dto)
        {
            // fake encrpt
            var encryptedContent = "encrypted_" + dto.Content;

            //hashing
            var bytes = Encoding.UTF8.GetBytes(dto.Content);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(bytes);
            var contentHash = Convert.ToBase64String(hashBytes);

            //entity
            var document = new Document
            {
                Title = dto.Title,
                EncryptedContent = encryptedContent,
                ContentHash = contentHash,
                OwnerUserId = dto.OwnerUserId
            };

            //db save
            _context.Documents.Add(document);
            
            _context.SaveChanges();

            return document.Id;
        }

        public int VerifyDocumentContent(int documentId, string contentToVerify)
        {
            var document = _context.Documents.Find(documentId);
            if (document == null)
            {
                return 0; // Document not found
            }
            var bytes = Encoding.UTF8.GetBytes(contentToVerify);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(bytes);
            var contentHashToVerify = Convert.ToBase64String(hashBytes);
            return document.ContentHash == contentHashToVerify ? 1 : -1; // 1: valid, -1: invalid
        }


        //GET /documents
        //Database → Entity → Service → Response DTO → Kullanıcı
        public List<DocumentResponseDto> GetDocuments()
        {
            var documents = _context.Documents.ToList();
            var response = documents.Select(d => new DocumentResponseDto
            {
                Id = d.Id,
                Title = d.Title,
                OwnerUserId = d.OwnerUserId,
                CreatedAt = d.CreatedAt
            }).ToList();
            return response;
        }

        public DocumentDetailResponseDto? GetDocumentById(int documentId)
        {
            var document = _context.Documents.Find(documentId);
            if (document == null)
            {
                return null;
            }
            return new DocumentDetailResponseDto
            {
                Id = document.Id,
                Title = document.Title,
                OwnerUserId = document.OwnerUserId,
                CreatedAt = document.CreatedAt,
                Content = document.EncryptedContent
            };
        }
    }
}
