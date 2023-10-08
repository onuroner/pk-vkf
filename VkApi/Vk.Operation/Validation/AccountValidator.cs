using FluentValidation;
using Vk.Schema;

namespace Vk.Operation.Validation;

public class CreateAccountValidator : AbstractValidator<AccountRequest>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Name).MinimumLength(20).WithMessage("Name length min value is 20.");
    }
}