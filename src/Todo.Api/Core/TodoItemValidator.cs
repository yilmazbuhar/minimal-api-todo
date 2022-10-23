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
        RuleFor(m => m.DueDate)
            .Must(BeAValidDate).WithMessage("Due date must be a valid date.")
            .GreaterThan(DateTime.Now);
    }

    private bool BeAValidDate(DateTime date)
    {
        return !date.Equals(default(DateTime));
    }
}

public class TodoItemUpdateModelValidator : AbstractValidator<TodoItemUpdateModel>
{
    public TodoItemUpdateModelValidator()
    {
        RuleFor(m => m.Title).NotEmpty();
        RuleFor(m => m.DueDate)
            .Must(BeAValidDate).WithMessage("Due date must be a valid date.")
            .GreaterThan(DateTime.Now);
    }

    private bool BeAValidDate(DateTime date)
    {
        return !date.Equals(default(DateTime));
    }
}