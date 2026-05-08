using FirstApi.Services;
using FirstApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiStatusController : ControllerBase
    {

        private readonly ProductService _productService;

        public ApiStatusController(ProductService productService) {
            _productService = productService;
        }


        [HttpGet(Name = "GetApiStatus")]
        public IActionResult Get() {
            return Ok(new {message = "Controller is working 🚀"});
        }


        [HttpGet("GetProducts")]
        public IActionResult GetProducts()
        {
            var products = _productService.GetProducts();
            return Ok(products);
        }


        [HttpPost(Name = "PostApiStatus")]
        public IActionResult Create([FromBody] ProductDto dto)
        {
            _productService.AddProduct(dto);

            return Ok(new
            {
                message = "Veri alındı",
                data = dto
            });
        }

    }
}
