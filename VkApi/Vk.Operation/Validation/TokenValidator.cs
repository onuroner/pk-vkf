using FluentValidation;
using Vk.Schema;

namespace Vk.Operation.Validation;

public class CreateTokenValidator : AbstractValidator<TokenRequest>
{

    public CreateTokenValidator()
    {
        RuleFor(x => x.CustomerNumber).NotEmpty().WithMessage("CustomerNumber is required.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(5).WithMessage("Password is required.");
    }
}