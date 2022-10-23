using AutoMapper;
using MediatR;

namespace Todo.App
{
    public class DeleteTodoItemCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, bool>
    {
        private readonly TodoDbContext _todoDbContext;
        private readonly IMapper _mapper;
        public DeleteTodoItemCommandHandler(TodoDbContext todoDbContext, IMapper mapper)
        {
            _todoDbContext = todoDbContext;
            _mapper = mapper;
        }

        public async Task<bool> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
        {
            //var todoItem = await _todoDbContext.TodoItem.FindAsync(request.Id);

            if (await _todoDbContext.TodoItem.FindAsync(request.Id) is TodoItem todo)
            {
                _todoDbContext.TodoItem.Remove(todo);
                await _todoDbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
