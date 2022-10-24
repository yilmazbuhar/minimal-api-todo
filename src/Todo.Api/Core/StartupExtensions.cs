using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace Todo.Api;

public static class StartupExtensions
{
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

        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        //services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddScoped<IValidator<TodoItemSaveModel>, TodoItemSaveModelValidator>();
        services.AddScoped<IValidator<TodoItemUpdateModel>, TodoItemUpdateModelValidator>();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    /// <summary>
    /// Exception handler
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.ContentType = Text.Plain;

                var exceptionHandlerFeature =
                    context.Features.Get<IExceptionHandlerFeature>();

                if (exceptionHandlerFeature?.Error is HttpRequestException)
                {
                    var exception = ((HttpRequestException)exceptionHandlerFeature.Error);
                    context.Response.StatusCode = (int)exception.StatusCode;
                    await context.Response.WriteAsync(exception.Message);
                }
                else if (exceptionHandlerFeature?.Error is BadHttpRequestException)
                {
                    var exception = ((BadHttpRequestException)exceptionHandlerFeature.Error);
                    context.Response.StatusCode = (int)exception.StatusCode;
                    await context.Response.WriteAsync(exception.Message);
                }
            });
        });

        return app;
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
            return await validator.Validate<TodoItemSaveModel>(todo, async () =>
            {
                Guid id = await mediator.Send(new SaveTodoItemCommand(todo));
                if (id != Guid.Empty)
                    return Results.Created($"/todoitem/{id}", null);

                return Results.StatusCode(500);
            });
        });

        app.MapPut("/todoitem/{id}", async (IValidator<TodoItemUpdateModel> validator, Guid id, TodoItemUpdateModel todo, IMediator mediator) =>
        {
            return await validator.Validate<TodoItemUpdateModel>(todo, async () =>
            {
                return await mediator.Send(new UpdateTodoItemCommand(id, todo)) ?
                    Results.Ok() : Results.UnprocessableEntity($"todo item with id {id} not found");

            });
        });

        app.MapPut("/todoitem/done/{id}", async (Guid id, IMediator mediator) =>
        {
            return await mediator.Send(new SetDoneTodoItemCommand() { Id = id }) ?
                Results.Ok() : Results.UnprocessableEntity($"todo item with id ({id}) not found");
        });

        app.MapGet("/todoitem/{onlyoverdue}", async (IMediator mediator, bool onlyoverdue) =>
        {
            return Results.Ok(await mediator.Send(new GetTodoItemsRequest(onlyoverdue)));
        });

        app.MapGet("/todoitem/detail/{id}", async (Guid id, IMediator mediator) =>
        {
            return Results.Ok(await mediator.Send(new GetTodoItemByIdRequest() { Id = id }));
        });

        app.MapDelete("/todoitem/{id}", async (Guid id, IMediator mediator) =>
        {
            return await mediator.Send(new DeleteTodoItemCommand() { Id = id }) ?
                Results.Ok() : Results.UnprocessableEntity($"todo item with id ({id}) not found");
        });

        return app;
    }
}
