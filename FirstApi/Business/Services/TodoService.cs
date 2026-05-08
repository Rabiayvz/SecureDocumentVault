using FirstApi.Dtos;

namespace FirstApi.services
{
    public class TodoService
    {
        private static List<TodoDto> _todos = new List<TodoDto>();

        public List<TodoDto> GetTodoList()
        {
            return _todos;
        }

        public void AddTodoItem(TodoDto dto)
        {
            _todos.Add(dto);
        }

    }
}
