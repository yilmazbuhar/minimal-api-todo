using AutoMapper;
using MediatR;

namespace Todo.Api
{
    public class UpdateTodoItemCommand : IRequest<bool>
    {
        public UpdateTodoItemCommand(Guid id, TodoItemUpdateModel todoItem)
        {
            TodoItem = todoItem;
            Id = id;
        }

        public TodoItemUpdateModel TodoItem { get; set; }
        public Guid Id { get; set; }
    }

    public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand, bool>
    {
        private readonly TodoDbContext _todoDbContext;
        private readonly IMapper _mapper;
        public UpdateTodoItemCommandHandler(TodoDbContext todoDbContext, IMapper mapper)
        {
            _todoDbContext = todoDbContext;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var todoItem = await _todoDbContext.TodoItem.FindAsync(request.Id);
            if (todoItem == null)
                return false;

            todoItem.Title = request.TodoItem.Title;
            todoItem.DueDate = request.TodoItem.DueDate;

            return await _todoDbContext.SaveChangesAsync() > 0;
        }
    }
}
