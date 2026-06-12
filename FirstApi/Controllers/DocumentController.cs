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

        [HttpGet]

        public IActionResult GetDocument(Guid id) {

    }
}
