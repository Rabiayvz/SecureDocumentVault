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
        private readonly CryptoService _cryptoService;
        private readonly HashService _hashService;

        public DocumentService(AppDbContext context, CryptoService cryptoService, HashService hashService)
        {
            _context = context;
            _cryptoService = cryptoService;
            _hashService = hashService;
        }

        //POST /documents
        //Kullanıcı → DTO → Service → Entity → Database
        public Guid CreateDocument(CreateDocumentDto dto, Guid requestingUserId)
        {
            // fake encrpt
            var encryptedContent = _cryptoService.Encrypt(dto.Content);
            var contentHash = _hashService.ComputeHash(dto.Content);

            //entity
            var document = new Document
            {
                Title = dto.Title,
                EncryptedContent = encryptedContent,
                ContentHash = contentHash,
                OwnerUserId = requestingUserId
            };

            //db save
            _context.Documents.Add(document);
            
            _context.SaveChanges();

            return document.Id;
        }

        public bool? VerifyDocumentContent(Guid documentId, Guid requestingUserId)
        {
            var document = _context.Documents.Find(documentId);
            if (document == null)
            {
                return null;
            }

            if (document.OwnerUserId != requestingUserId)
            {
                throw new UnauthorizedAccessException("Bu belgeye erişim yetkiniz yok.");
            }

            var decryptedContent = _cryptoService.Decrypt(document.EncryptedContent);

            return _hashService.VerifyHash(decryptedContent, document.ContentHash);

        }


        //GET /documents
        //Database → Entity → Service → Response DTO → Kullanıcı
        public List<DocumentResponseDto> GetDocuments(Guid requestingUserId)
        {
            var documents = _context.Documents
                .Where(d => d.OwnerUserId == requestingUserId)
                .ToList();

            var response = documents.Select(d => new DocumentResponseDto
            {
                Id = d.Id,
                Title = d.Title,
                OwnerUserId = d.OwnerUserId,
                CreatedAt = d.CreatedAt
            }).ToList();
            return response;
        }

        public DocumentDetailResponseDto? GetDocumentById(Guid documentId, Guid requestingUserId)
        {
            var document = _context.Documents.Find(documentId);
            if (document == null)
            {
                return null;
            }

            if (document.OwnerUserId != requestingUserId)
            {
                throw new UnauthorizedAccessException("Bu belgeye erişim yetkiniz yok.");
            }

            var decryptedContent = _cryptoService.Decrypt(document.EncryptedContent);

            return new DocumentDetailResponseDto
            {
                Id = document.Id,
                Title = document.Title,
                OwnerUserId = document.OwnerUserId,
                CreatedAt = document.CreatedAt,
                Content = decryptedContent
            };
        }
    }
}
