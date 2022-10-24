using AutoMapper;
using MediatR;

namespace Todo.Api;

public class SaveTodoItemCommand : IRequest<Guid>
{
    public SaveTodoItemCommand(TodoItemSaveModel todoItem)
    {
        TodoItem = todoItem;
    }

    public TodoItemSaveModel TodoItem { get; set; }
}

public class SaveTodoItemCommandHandler : IRequestHandler<SaveTodoItemCommand, Guid>
{
    private readonly TodoDbContext _todoDbContext;
    private readonly IMapper _mapper;
    public SaveTodoItemCommandHandler(TodoDbContext todoDbContext, IMapper mapper)
    {
        _todoDbContext = todoDbContext;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(SaveTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = _mapper.Map<TodoItem>(request.TodoItem);
        if (todoItem == null) 
            return Guid.Empty;

        await _todoDbContext.TodoItem.AddAsync(todoItem);
        await _todoDbContext.SaveChangesAsync();

        return todoItem.Id;
    }
}
