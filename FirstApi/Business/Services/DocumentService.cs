using FirstApi.Data;
using FirstApi.Dtos;
using FirstApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FirstApi.Business.Services
{
    public class DocumentService
    {
        private readonly AppDbContext _context;
        private readonly CryptoService _cryptoService;
        private readonly HashService _hashService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuditLogService _auditLogService;


        public DocumentService(AppDbContext context, CryptoService cryptoService, HashService hashService, IHttpContextAccessor httpContextAccessor, AuditLogService auditLogService)
        {
            _context = context;
            _cryptoService = cryptoService;
            _hashService = hashService;
            _httpContextAccessor = httpContextAccessor;
            _auditLogService = auditLogService;
        }

        private Guid GetCurrentUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) throw new UnauthorizedAccessException("User not found in token.");
            return Guid.Parse(claim.Value);
        }

        private string GetCurrentUserRole()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role);
            if (claim == null) throw new UnauthorizedAccessException("Role not found in token.");
            return claim.Value;
        }

        //POST /documents
        //Kullanıcı → DTO → Service → Entity → Database
        public Guid CreateDocument(CreateDocumentDto dto)
        {
            var userId = GetCurrentUserId();

            var encryptedContent = _cryptoService.Encrypt(dto.Content);
            var contentHash = _hashService.ComputeHash(dto.Content);

            //entity
            var document = new Document
            {
                Title = dto.Title,
                EncryptedContent = encryptedContent,
                ContentHash = contentHash,
                OwnerUserId = userId
            };

            //db save
            _context.Documents.Add(document);
            _context.SaveChanges();
            _auditLogService.Log("DocumentCreated", $"Document '{document.Title}' created. Id: {document.Id}");
            return document.Id;
        }

        public bool? VerifyDocumentContent(Guid documentId)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            
            var document = _context.Documents.Find(documentId);
            if (document == null) return null;

            // User sadece kendi belgesini verify edebilir
            if (role == "User" && document.OwnerUserId != userId)
                throw new UnauthorizedAccessException("You can only verify your own documents.");

            // Manager sadece takımının belgelerini verify edebilir
            if (role == "Manager")
            {
                var teamUserIds = _context.Users
                    .Where(u => u.ManagerId == userId)
                    .Select(u => u.Id)
                    .ToList();

                teamUserIds.Add(userId);

                if (!teamUserIds.Contains(document.OwnerUserId))
                    throw new UnauthorizedAccessException("You can only verify your team's documents.");
            }

            var decryptedContent = _cryptoService.Decrypt(document.EncryptedContent);
            var result = _hashService.VerifyHash(decryptedContent, document.ContentHash);

            _auditLogService.Log("DocumentVerified", $"Document '{document.Title}' verified. Id: {documentId}. Result: {result}");

            return result;

        }


        //GET /documents
        //Database → Entity → Service → Response DTO → Kullanıcı
        public List<DocumentResponseDto> GetDocuments()
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            IQueryable<Document> query = _context.Documents;

            if (role == "Admin")
            {
                // Admin her şeyi görür, filtre yok
            }
            else if (role == "Manager")
            {
                var teamUserIds = _context.Users
                    .Where(u => u.ManagerId == userId)
                    .Select(u => u.Id)
                    .ToList();

                teamUserIds.Add(userId); // kendi belgelerini de görür

                query = query.Where(d => teamUserIds.Contains(d.OwnerUserId));
            }
            else
            {
                // User ve Auditor: sadece kendi belgeleri
                query = query.Where(d => d.OwnerUserId == userId);
            }

            return query.Select(d => new DocumentResponseDto
            {
                Id = d.Id,
                Title = d.Title,
                OwnerUserId = d.OwnerUserId,
                CreatedAt = d.CreatedAt
            }).ToList();
        }

        public DocumentDetailResponseDto? GetDocumentById(Guid documentId)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            
            var document = _context.Documents.Find(documentId);
            if (document == null) return null;

            // Auditor içerik okuyamaz
            if (role == "Auditor")
            {
                _auditLogService.Log("UnauthorizedAccess", $"Unauthorized attempt to view document {documentId}");
                throw new UnauthorizedAccessException("Auditors cannot read document content.");
            }

            // User sadece kendi belgesini okuyabilir
            if (role == "User" && document.OwnerUserId != userId)
            {
                _auditLogService.Log("UnauthorizedAccess", $"Unauthorized attempt to view document {documentId}");
                throw new UnauthorizedAccessException("You can only access your own documents.");
            }

            // Manager sadece takımının belgelerini okuyabilir
            if (role == "Manager")
            {
                var teamUserIds = _context.Users
                    .Where(u => u.ManagerId == userId)
                    .Select(u => u.Id)
                    .ToList();

                teamUserIds.Add(userId);

                if (!teamUserIds.Contains(document.OwnerUserId))
                {
                    _auditLogService.Log("UnauthorizedAccess", $"Unauthorized attempt to view document {documentId}");
                    throw new UnauthorizedAccessException("You can only access your team's documents.");
                }
            }
            _auditLogService.Log("DocumentViewed", $"Document '{document.Title}' viewed. Id: {documentId}");
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
