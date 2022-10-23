using AutoMapper;
using MediatR;

namespace Todo.Api
{
    public class SaveTodoItemCommand : IRequest<bool>
    {
        public SaveTodoItemCommand(TodoItemSaveModel todoItem)
        {
            TodoItem = todoItem;
        }

        public TodoItemSaveModel TodoItem { get; set; }
    }

    public class SaveTodoItemCommandHandler : IRequestHandler<SaveTodoItemCommand, bool>
    {
        private readonly TodoDbContext _todoDbContext;
        private readonly IMapper _mapper;
        public SaveTodoItemCommandHandler(TodoDbContext todoDbContext, IMapper mapper)
        {
            _todoDbContext = todoDbContext;
            _mapper = mapper;
        }

        public async Task<bool> Handle(SaveTodoItemCommand request, CancellationToken cancellationToken)
        {
            var todoItem = _mapper.Map<TodoItem>(request.TodoItem);
            if (todoItem == null) 
                return false;

            await _todoDbContext.TodoItem.AddAsync(todoItem);
            var affectedRow = await _todoDbContext.SaveChangesAsync();

            return affectedRow > 0;
        }
    }
}
