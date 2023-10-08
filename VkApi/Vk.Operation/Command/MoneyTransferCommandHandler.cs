using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vk.Base.Response;
using Vk.Base.Transaction;
using Vk.Data.Context;
using Vk.Data.Domain;
using Vk.Schema;

namespace Vk.Operation.Command;

public class MoneyTransferCommandHandler :
    IRequestHandler<CreateMoneyTransferCommand, ApiResponse<MoneyTransferResponse>>
{
    private readonly VkDbContext dbContext;
    private readonly IMapper mapper;

    public MoneyTransferCommandHandler(VkDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }


    public async Task<ApiResponse<MoneyTransferResponse>> Handle(CreateMoneyTransferCommand request,
        CancellationToken cancellationToken)
    {

        if (request.Model.FromAccountId == request.Model.ToAccountId)
        {
            return new ApiResponse<MoneyTransferResponse>("Accounts cannot be same");
        }

        string refNumber = Guid.NewGuid().ToString().Replace("-", "").ToLower();
        
        var checkFromAccount = await CheckAccount(request.Model.FromAccountId, cancellationToken);
        var checkToAccount = await CheckAccount(request.Model.ToAccountId, cancellationToken); 
        if (!checkFromAccount.Success)
        {
            return new ApiResponse<MoneyTransferResponse>(checkFromAccount.Message);
        }
        if (!checkToAccount.Success)
        {
            return new ApiResponse<MoneyTransferResponse>(checkToAccount.Message);
        }

        var balanceFrom = await BalanceOperation(request.Model.FromAccountId, request.Model.Amount,
            TransactionDirection.Credit, cancellationToken);
        var balanceTo = await BalanceOperation(request.Model.ToAccountId, request.Model.Amount,
            TransactionDirection.Debit, cancellationToken);
       
        if (!balanceFrom.Success)
        {
            return new ApiResponse<MoneyTransferResponse>(balanceFrom.Message);
        }
        if (!balanceTo.Success)
        {
            return new ApiResponse<MoneyTransferResponse>(balanceTo.Message);
        }

        Account from = checkFromAccount.Response;
        Account to = checkToAccount.Response;

        string txnCode = from.CustomerId == to.CustomerId ? "Virement" : "Remittance";
       
        AccountTransaction transactionFrom = new AccountTransaction();
        transactionFrom.TransactionDate =  DateTime.UtcNow;
        transactionFrom.AccountId = from.Id;
        transactionFrom.TransactionCode = txnCode;
        transactionFrom.IsActive = true;
        transactionFrom.Description = request.Model.Description;
        transactionFrom.CreditAmount = request.Model.Amount;
        transactionFrom.ReferenceNumber = refNumber;
        
        AccountTransaction transactionTo = new AccountTransaction();
        transactionTo.TransactionDate =  DateTime.UtcNow;
        transactionTo.AccountId = to.Id;
        transactionTo.TransactionCode = txnCode;
        transactionTo.IsActive = true;
        transactionTo.Description = request.Model.Description;
        transactionTo.DebitAmount = request.Model.Amount;
        transactionTo.ReferenceNumber = refNumber;

        await dbContext.Set<AccountTransaction>().AddAsync(transactionFrom, cancellationToken);
        await dbContext.Set<AccountTransaction>().AddAsync(transactionTo, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);


        var response = mapper.Map<MoneyTransferResponse>(request.Model);
        response.ReferenceNumber = refNumber;
        response.TransactionCode = txnCode;
        response.TransactionDate = DateTime.UtcNow;

        return new ApiResponse<MoneyTransferResponse>(response);
    }

    private async Task<ApiResponse<Account>> CheckAccount(int accountId, CancellationToken cancellationToken)
    {
        var account = await dbContext.Set<Account>().Where(x => x.Id == accountId).FirstOrDefaultAsync(cancellationToken);

        if (account == null)
        {
            return new ApiResponse<Account>("Invalid Account");
        }

        if (!account.IsActive)
        {
            return new ApiResponse<Account>("Invalid Account");
        }

        return new ApiResponse<Account>(account);
    }
    
    private async Task<ApiResponse> BalanceOperation(int accountId,decimal amount,TransactionDirection direction, CancellationToken cancellationToken)
    {
        var account = await dbContext.Set<Account>().Where(x => x.Id == accountId).FirstOrDefaultAsync(cancellationToken);

        if (direction == TransactionDirection.Credit)
        {
            if (account.Balance < amount)
            {
                return new ApiResponse("Insufficent balance");
            }
            account.Balance -= amount;
        }
        if (direction == TransactionDirection.Debit)
        {
            account.Balance += amount;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }
    
}