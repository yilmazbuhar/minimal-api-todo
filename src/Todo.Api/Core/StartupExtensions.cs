using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Todo.App;
using FluentValidation.Results;
using System.ComponentModel.DataAnnotations;

namespace Todo.Api
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Validate object that given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="validator"></param>
        /// <param name="model"></param>
        /// <param name="endpointFunc"></param>
        /// <returns></returns>
        public static async Task<IResult> Validate<T>(this IValidator<T> validator, T model, Func<Task<IResult>> endpointFunc)
        {
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            return await endpointFunc();
        }

        /// <summary>
        /// Adds startup services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TodoDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });
            services.AddMediatR(typeof(Program));
            services.AddAutoMapper(typeof(Program));
            services.AddValidatorsFromAssemblyContaining<Program>();
            //services.AddScoped<IValidator<TodoItemSaveModel>, TodoItemValidator>();
            services.AddValidatorsFromAssemblyContaining<Program>();

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        /// <summary>
        /// Use swagger UI
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication UseSwaggerEndPoints(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            return app;
        }

        /// <summary>
        /// Adds a RouteEndpoints to the IEndpointRouteBuilder
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static WebApplication RegisterEndPoints(this WebApplication app)
        {
            app.MapPost("/todoitem", async (IValidator<TodoItemSaveModel> validator, TodoItemSaveModel todo, IMediator mediator) =>
            {
                await validator.Validate<TodoItemSaveModel>(todo, async () =>
                {
                    await mediator.Send(new SaveTodoItemCommand(todo));

                    return Results.Created($"/todoitem", null);
                });
            });

            app.MapPut("/todoitem/{id}", async (IValidator<TodoItemUpdateModel> validator, Guid id, TodoItemUpdateModel todo, IMediator mediator) =>
            {
                await validator.Validate<TodoItemUpdateModel>(todo, async () =>
                {
                    return await mediator.Send(new UpdateTodoItemCommand(id, todo)) ?
                        Results.Ok() : Results.UnprocessableEntity($"todo item with id {id} not found");

                });
            });

            app.MapPut("/todoitem/done/{id}", async (Guid id, IMediator mediator) =>
            {
                return await mediator.Send(new SetDoneTodoItemCommand() { Id = id }) ?
                    Results.Ok() : Results.UnprocessableEntity($"todo item with id {id} not found");
            });

            app.MapGet("/todoitem/{overdue}", async (bool overdue, IMediator mediator) =>
            {
                return await mediator.Send(new GetTodoItemsRequest(overdue));
            });

            app.MapDelete("/todoitem/{id}", async (Guid id, IMediator mediator) =>
            {
                await mediator.Send(new DeleteTodoItemCommand() { Id = id });
            });

            return app;
        }
    }
}
