using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vk.Base.Response;
using Vk.Data.Context;
using Vk.Data.Domain;
using Vk.Data.Uow;
using Vk.Schema;

namespace Vk.Operation;

public class MoneyTransferQueryHandler :
    IRequestHandler<GetMoneyTransferByReferenceQuery, ApiResponse<List<AccountTransactionResponse>>>,
    IRequestHandler<GetMoneyTransferByAccountIdQuery, ApiResponse<List<AccountTransactionResponse>>>,
    IRequestHandler<GetMoneyTransferByParametersQuery, ApiResponse<List<AccountTransactionResponse>>>
{
    private readonly VkDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public MoneyTransferQueryHandler(VkDbContext dbContext, IMapper mapper, IUnitOfWork unitOfWork)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }


    public async Task<ApiResponse<List<AccountTransactionResponse>>> Handle(GetMoneyTransferByReferenceQuery request,
        CancellationToken cancellationToken)
    {
        var list = await dbContext.Set<AccountTransaction>().Include(x => x.Account).ThenInclude(x => x.Customer)
            .Where(x => x.ReferenceNumber == request.ReferenceNumber).ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<AccountTransactionResponse>>(list);
        return new ApiResponse<List<AccountTransactionResponse>>(mapped);
    }

    public async Task<ApiResponse<List<AccountTransactionResponse>>> Handle(GetMoneyTransferByAccountIdQuery request,
        CancellationToken cancellationToken)
    {
        var list = await dbContext.Set<AccountTransaction>().Include(x => x.Account).ThenInclude(x => x.Customer)
            .Where(x => x.AccountId == request.AccountId).ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<AccountTransactionResponse>>(list);
        return new ApiResponse<List<AccountTransactionResponse>>(mapped);
    }

    public async Task<ApiResponse<List<AccountTransactionResponse>>> Handle(GetMoneyTransferByParametersQuery request,
        CancellationToken cancellationToken)
    {
        var predicate = PredicateBuilder.New<AccountTransaction>(true);
        if (request.accountId > 0)
            predicate.And(x => x.AccountId == request.accountId);
        if (request.customerId > 0)
            predicate.And(x => x.Account.CustomerId == request.customerId);
        if (!string.IsNullOrWhiteSpace(request.description))
            predicate.And(x => x.Description.Contains(request.description));
        if (request.minAmount > 0)
            predicate.And(x => x.CreditAmount >= request.minAmount || x.DebitAmount >= request.minAmount);
        if (request.maxAmount > 0)
            predicate.And(x => x.CreditAmount <= request.maxAmount || x.DebitAmount <= request.maxAmount);
        if (request.beginDate != null)
            predicate.And(x => x.TransactionDate >= request.beginDate);
        if (request.endDate != null)
            predicate.And(x => x.TransactionDate <= request.endDate);
        
        var list = await dbContext.Set<AccountTransaction>()
            .Include(x => x.Account).ThenInclude(x => x.Customer)
            .Where(predicate).ToListAsync(cancellationToken);
      
        var list2 = unitOfWork.AccountTransactionRepository.Where(predicate,"Account.Customer").ToList();

        var mapped = mapper.Map<List<AccountTransactionResponse>>(list);
        return new ApiResponse<List<AccountTransactionResponse>>(mapped);
    }
}