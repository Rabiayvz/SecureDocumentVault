using FirstApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiStatusController : ControllerBase
    {

        private static List<ProductDto> _products = new List<ProductDto>();

        [HttpGet(Name = "GetApiStatus")]

        public IActionResult Get() {
            return Ok(new {message = "Controller is working 🚀"});
        }

        [HttpGet("GetProducts")]

        public IActionResult GetProducts()
        {
            return Ok(_products);
        }

        [HttpPost(Name = "PostApiStatus")]

        public IActionResult Create([FromBody] ProductDto dto)
        {
            _products.Add(dto);

            return Ok(new
            {
                message = "Veri alındı",
                data = dto
            });
        }

    }
}
