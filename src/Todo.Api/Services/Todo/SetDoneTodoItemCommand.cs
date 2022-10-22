using AutoMapper;
using MediatR;

namespace Todo.App
{
    public class SetDoneTodoItemCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class SetDoneTodoItemCommandHandler : IRequestHandler<SetDoneTodoItemCommand, bool>
    {
        private readonly TodoDbContext _todoDbContext;
        private readonly IMapper _mapper;
        public SetDoneTodoItemCommandHandler(TodoDbContext todoDbContext, IMapper mapper)
        {
            _todoDbContext = todoDbContext;
            _mapper = mapper;
        }

        public async Task<bool> Handle(SetDoneTodoItemCommand request, CancellationToken cancellationToken)
        {
            var todoItem = await _todoDbContext.TodoItem.FindAsync(request.Id);
            if (todoItem == null)
                return false;

            todoItem.Done = true;

            return await _todoDbContext.SaveChangesAsync() > 0;
        }
    }
}
