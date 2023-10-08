using FluentValidation;
using Vk.Schema;

namespace Vk.Operation.Validation;

public class CreateEftTransactionValidator : AbstractValidator<EftTransactionRequest>
{
    public CreateEftTransactionValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.ReceiverName).NotEmpty();
        RuleFor(x => x.ReceiverAddress).NotEmpty();
        RuleFor(x => x.ReceiverAddressType).NotEmpty();
        RuleFor(x => x.Amount).NotEmpty();
        RuleFor(x => x.ChargeAmount).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.TransactionCode).NotEmpty();
        RuleFor(x => x.TransactionDate).NotEmpty();
    }
}