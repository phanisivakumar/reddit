using FluentValidation;

namespace Analytics.PubSub.Model;

public class Validator<T> : AbstractValidator<string>
{
    public Validator()
    {
        RuleFor(message => message)
            .NotEmpty()
            .WithMessage("Message cannot be empty.")
            .MaximumLength(100)
            .WithMessage("Message cannot exceed 100 characters."); // Add any additional validation rules as needed.
    }
}