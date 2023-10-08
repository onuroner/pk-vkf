using FluentValidation;
using Vk.Schema;

namespace Vk.Operation.Validation;

public class CreateCardValidator : AbstractValidator<CardRequest>
{

    public CreateCardValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.CardHolder).NotEmpty();
        RuleFor(x => x.CardNumber).NotEmpty();
        RuleFor(x => x.Cvv).NotEmpty();
        RuleFor(x => x.ExpiryDate).NotEmpty();
        RuleFor(x => x.ExpenseLimit).NotEmpty();
    }
}