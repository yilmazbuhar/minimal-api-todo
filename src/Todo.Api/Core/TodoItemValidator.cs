using FluentValidation;

public class TodoItemValidator : AbstractValidator<TodoItem>
{
    public TodoItemValidator()
    {
        RuleFor(m => m.Title).NotEmpty();
        RuleFor(m => m.DueDate).GreaterThan(DateTime.Now);
    }
}
