using FirstApi.Dtos;
using FirstApi.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("documents")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;
        public DocumentController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        private Guid GetRequestingUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        [HttpPost]
        public IActionResult CreateDocument(CreateDocumentDto dto)
        {
            var requestingUserId = GetRequestingUserId();
            var documentId = _documentService.CreateDocument(dto, requestingUserId);

            return Ok(documentId);
        }

        [HttpGet]
        public IActionResult GetDocuments()
        {
            var requestingUserId = GetRequestingUserId();
            var documents = _documentService.GetDocuments(requestingUserId);

            return Ok(documents);
        }

        [HttpGet("{id}")]
        public IActionResult GetDocumentById(Guid id)
        {
            var requestingUserId = GetRequestingUserId();

            try
            {
                var document = _documentService.GetDocumentById(id, requestingUserId);

                if (document == null)
                {
                    return NotFound();
                }

                return Ok(document);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

        [HttpPost("{id}/verify")]
        public IActionResult VerifyDocument(Guid id)
        {
            var requestingUserId = GetRequestingUserId();

            try
            {
                var result = _documentService.VerifyDocumentContent(id, requestingUserId);

                if (result == null)
                {
                    return NotFound();
                }

                if (result.Value)
                {
                    return Ok("Document is valid");
                }

                return Ok("Document is invalid");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }
    }
}