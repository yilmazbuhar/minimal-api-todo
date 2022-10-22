using FluentValidation;
using FluentValidation.Results;
using System.Reflection;
using System.Linq;
using Todo.App;

public class TodoItemValidator : AbstractValidator<TodoItemSaveModel>
{
    public TodoItemValidator()
    {
        RuleFor(m => m.Title).NotEmpty();
        RuleFor(m => m.DueDate).GreaterThan(DateTime.Now);
    }
}