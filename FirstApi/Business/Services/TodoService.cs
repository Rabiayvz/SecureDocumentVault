using FirstApi.Data;
using FirstApi.Dtos;
using FirstApi.Models;

namespace FirstApi.Services
{
    public class TodoService
    {
        private readonly AppDbContext _context;

        public TodoService(AppDbContext context)
        {
            _context = context;
        }
        public List<Todo> GetTodoList()
        {
            return _context.Todos.ToList();
        }

        public void AddTodoItem(TodoDto dto)
        {
            var todo = new Todo
            {
                Title = dto.Title,
                IsCompleted = dto.isCompleted
            };

            _context.Todos.Add(todo);

            _context.SaveChanges();
        }

    }
}
