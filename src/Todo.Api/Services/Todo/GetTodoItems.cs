using MediatR;
using System.Data.Entity;

namespace minimal_web_api.Services.Todo
{
    public class GetTodoItemsRequest : IRequest<List<TodoItem>>
    {
        public bool OnlyOverdue { get; set; }
    }

    public class GetTodoItemsRequestHandler : IRequestHandler<GetTodoItemsRequest, List<TodoItem>>
    {
        private readonly TodoDbContext _todoDbContext;
        public GetTodoItemsRequestHandler(TodoDbContext todoDbContext)
        {
            _todoDbContext = todoDbContext; 
        }

        public async Task<List<TodoItem>> Handle(GetTodoItemsRequest request, CancellationToken cancellationToken)
        {
            return await _todoDbContext.TodoItem.Where(ti => !ti.Done && (request.OnlyOverdue && ti.DueDate < DateTime.Now)).ToListAsync();
        }
    }
}
