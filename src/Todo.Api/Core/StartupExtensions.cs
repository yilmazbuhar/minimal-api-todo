using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Todo.Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddDbContext<TodoDbContext>(opt =>
            {
                opt.UseNpgsql("Host=localhost;Database=todoitems;Username=user;Password=5EPZK8rk9$mLJ)d@");
            });
            services.AddAutoMapper(typeof(Program));
            services.AddValidatorsFromAssemblyContaining<Program>();
            services.AddScoped<IValidator<TodoItem>, TodoItemValidator>();

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static WebApplication AddSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;
        }

        public static WebApplication RegisterEndPoints(this WebApplication app)
        {
            app.MapPost("/todoitem", async (Validator<TodoItem> validator, TodoItem todo, TodoDbContext db) =>
            {
                var (isValid, value, errors) = validator;
                if (!isValid)
                    return Results.BadRequest(errors);

                db.TodoItem.FirstOrDefault();
                await db.SaveChangesAsync();

                return Results.Created($"/todoitem/{todo.Id}", todo);
            });

            app.MapPut("/todoitem", async (TodoItem todo, TodoDbContext db) =>
            {
                var todoItem = await db.TodoItem.SingleOrDefaultAsync(ti => ti.Id == todo.Id);
                if (todoItem == null)
                    return Results.UnprocessableEntity($"todo item with id {todo.Id} not found");

                todoItem.Title = todo.Title;
                todoItem.DueDate = todo.DueDate;

                await db.SaveChangesAsync();

                return Results.Created($"/todoitem/{todo.Id}", todo);
            });

            app.MapPut("/todoitem/done/{id}", async (Guid id, TodoDbContext db) =>
            {
                var todoItem = await db.TodoItem.FindAsync(id);
                if (todoItem == null)
                    return Results.UnprocessableEntity($"todo item with id {id} not found");

                todoItem.Done = true;

                await db.SaveChangesAsync();

                return Results.Ok();
            });

            app.MapGet("/todoitem", async (TodoDbContext db) =>
                await db.TodoItem.Where(ti => !ti.Done).ToListAsync());


            app.MapGet("/todoitem/overdue", async (TodoDbContext db) =>
                await db.TodoItem.Where(ti => ti.DueDate < DateTime.Now && !ti.Done).ToListAsync());

            app.MapDelete("/todoitem/{id}", async (Guid id, TodoDbContext db) =>
            {
                if (await db.TodoItem.FindAsync(id) is TodoItem todo)
                {
                    db.TodoItem.Remove(todo);
                    await db.SaveChangesAsync();

                    return Results.Ok(todo);
                }

                return Results.NotFound();
            });

            return app;
        }
    }
}
