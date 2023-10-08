using FluentValidation;
using Vk.Schema;

namespace Vk.Operation.Validation;

public class CreateCustomerValidator : AbstractValidator<CustomerRequest>
{

    public CreateCustomerValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Firstname is required.");
        RuleFor(x => x.FirstName).MinimumLength(20).WithMessage("Firstname length min value is 20.");
        
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.");
        RuleFor(x => x.Email).MinimumLength(20).WithMessage("Email length min value is 20.");
        
        RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required.");
        RuleFor(x => x.LastName).MinimumLength(20).WithMessage("LastName length min value is 20.");
    }
}