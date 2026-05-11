using FirstApi.Dtos;
using FirstApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;
        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public IActionResult GetTodos()
        {
            return Ok(_todoService.GetTodoList());
        }

        [HttpPost]
        public IActionResult AddTodo([FromBody] TodoDto dto)
        {
            _todoService.AddTodoItem(dto);

            return Ok(new
            {
                message = "Todo eklendi",
                data = dto
            });
        }

    }
}
