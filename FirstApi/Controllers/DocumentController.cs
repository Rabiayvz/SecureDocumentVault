using FirstApi.Dtos;
using FirstApi.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,User")]
        public IActionResult CreateDocument(CreateDocumentDto dto)
        {
            var documentId = _documentService.CreateDocument(dto);
            return Ok(documentId);
        }

        [HttpGet]
        public IActionResult GetDocuments()
        {
            var documents = _documentService.GetDocuments();
            return Ok(documents);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        public IActionResult GetDocumentById(Guid id)
        {
            var document = _documentService.GetDocumentById(id);
            if (document == null) return NotFound();
            return Ok(document);
        }

        [HttpPost("{id}/verify")]
        public IActionResult VerifyDocument(Guid id)
        {
            var result = _documentService.VerifyDocumentContent(id);
            if (result == null) return NotFound();
            return Ok(result.Value ? "Document is valid" : "Document is invalid");
        }
    }
}