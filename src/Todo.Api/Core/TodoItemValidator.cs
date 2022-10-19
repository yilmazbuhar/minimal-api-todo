using FluentValidation;
using FluentValidation.Results;
using System.Reflection;
using System.Linq;


public class TodoItemValidator : AbstractValidator<TodoItem>
{
    public TodoItemValidator()
    {
        RuleFor(m => m.Title).NotEmpty();
        RuleFor(m => m.DueDate).GreaterThan(DateTime.Now);
    }
}


public class Validator<T>
{
    private ValidationResult Validation { get; }

    private Validator(T model, ValidationResult validation)
    {
        Model = model;
        Validation = validation;
    }

    public T Model { get; }
    public bool IsValid => Validation.IsValid;

    public IDictionary<string, string[]> Errors =>
        Validation
            .Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(x => x.Key, x => x.Select(e => e.ErrorMessage).ToArray());

    public void Deconstruct(out bool isValid, out T model, out List<string> errors)
    {
        isValid = IsValid;
        model = Model;
        errors = Errors.Select(err => $"{err.Key} => {string.Join(",", err.Value)}").ToList();
    }

    public static async ValueTask<Validator<T>> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var value = await context.Request.ReadFromJsonAsync<T>();
        var validator = context.RequestServices.GetRequiredService<IValidator<T>>();

        if (value is null)
        {
            throw new ArgumentException(parameter.Name);
        }

        var results = await validator.ValidateAsync(value);

        return new Validator<T>(value, results);
    }
}
