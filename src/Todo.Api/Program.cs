// Roadmap
 // Apply validator to all endpoints
 // Implement CQRS with MediatR
 // Implement logging
 // Implement opentelemetry OR APM

using Todo.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServices();

var app = builder.Build();

app.UseSwaggerEndPoints()
    .RegisterEndPoints();

app.Run();


