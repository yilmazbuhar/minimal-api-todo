using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Todo.App
{
    public class GetTodoItemsRequest : IRequest<List<TodoItem>>
    {
        public bool OnlyOverdue { get; set; }

        public GetTodoItemsRequest(bool onlyOverdue)
        {
            OnlyOverdue = onlyOverdue;
        }
    }

    public class GetTodoItemByIdRequest : IRequest<TodoItem>
    {
        public Guid Id { get; set; }
    }

    public class GetTodoItemsRequestHandler :
        IRequestHandler<GetTodoItemsRequest, List<TodoItem>>,
        IRequestHandler<GetTodoItemByIdRequest, TodoItem>
    {
        private readonly TodoDbContext _todoDbContext;
        public GetTodoItemsRequestHandler(TodoDbContext todoDbContext)
        {
            _todoDbContext = todoDbContext;
        }

        public async Task<List<TodoItem>> Handle(GetTodoItemsRequest request, CancellationToken cancellationToken)
        {
            var query = _todoDbContext.TodoItem.Where(ti => !ti.Done);
            if (request.OnlyOverdue)
                query = query.Where(ti => ti.DueDate < DateTime.Now);

            return await query.ToListAsync();
        }

        public async Task<TodoItem> Handle(GetTodoItemByIdRequest request, CancellationToken cancellationToken) =>
                await _todoDbContext.TodoItem.FindAsync(request.Id);
    }
}
