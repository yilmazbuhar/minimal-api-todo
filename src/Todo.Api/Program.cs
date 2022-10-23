using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Todo.Api;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServices(builder.Configuration);
//builder.Services.Configure<ApiBehaviorOptions>(options =>
//{
//    options.SuppressModelStateInvalidFilter = true;
//});

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwaggerEndPoints()
    .RegisterEndPoints();

app.Run();


