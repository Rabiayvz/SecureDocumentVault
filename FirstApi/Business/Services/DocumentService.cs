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

        public List<DocumentResponseDto> GetDocuments(int documentId)
        {
            var document = _context.Documents.Find(documentId);
            if (document == null)
            {
                throw new Exception("Document not found");
            }
            return document.Id;
        }
    }
}
