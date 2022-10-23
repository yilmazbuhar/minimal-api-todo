using FluentValidation;
using FluentValidation.Results;
using System.Reflection;
using System.Linq;
using Todo.App;

namespace Todo.Api;

public class TodoItemSaveModelValidator : AbstractValidator<TodoItemSaveModel>
{
    public TodoItemSaveModelValidator()
    {
        RuleFor(m => m.Title).NotEmpty();
        RuleFor(m => m.DueDate).GreaterThan(DateTime.Now);
    }
}

public class TodoItemUpdateModelValidator : AbstractValidator<TodoItemUpdateModel>
{
    public TodoItemUpdateModelValidator()
    {
        RuleFor(m => m.Title).NotEmpty();
        RuleFor(m => m.DueDate).GreaterThan(DateTime.Now);
    }
}