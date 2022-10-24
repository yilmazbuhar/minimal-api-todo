using FluentValidation;

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

public static class ValidationExtension
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
}