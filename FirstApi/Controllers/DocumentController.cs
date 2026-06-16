using FirstApi.Dtos;
using FirstApi.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("documents")]
    public class DocumentController : ControllerBase
    {
        private readonly DocumentService _documentService;
        public DocumentController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost]
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
        public IActionResult GetDocumentById(Guid id)
        {
            var document = _documentService.GetDocumentById(id);

            if (document == null)
            {
                return NotFound();
            }

            return Ok(document);
        }

        [HttpPost("{id}/verify")]
        public IActionResult VerifyDocument(Guid id, VerifyDocumentDto dto)
        {
            var result = _documentService.VerifyDocumentContent(id, dto.Content);

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


    }

}
