using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vk.Base.Response;
using Vk.Data.Context;
using Vk.Data.Domain;
using Vk.Schema;

namespace Vk.Operation;

public class CardQueryHandler :
    IRequestHandler<GetAllCardQuery, ApiResponse<List<CardResponse>>>,
    IRequestHandler<GetCardByIdQuery, ApiResponse<CardResponse>>,
    IRequestHandler<GetCardByCustomerIdQuery, ApiResponse<List<CardResponse>>>,
    IRequestHandler<GetCardByAccountIdQuery, ApiResponse<CardResponse>>
{
    private readonly VkDbContext dbContext;
    private readonly IMapper mapper;

    public CardQueryHandler(VkDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<CardResponse>>> Handle(GetAllCardQuery request,
        CancellationToken cancellationToken)
    {
        List<Card> list = await dbContext.Set<Card>()
            .Include(x => x.Account)
            .ToListAsync(cancellationToken);

        List<CardResponse> mapped = mapper.Map<List<CardResponse>>(list);
        return new ApiResponse<List<CardResponse>>(mapped);
    }

    public async Task<ApiResponse<CardResponse>> Handle(GetCardByIdQuery request,
        CancellationToken cancellationToken)
    {
        Card? entity = await dbContext.Set<Card>()
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return new ApiResponse<CardResponse>("Record not found!");
        }

        CardResponse mapped = mapper.Map<CardResponse>(entity);
        return new ApiResponse<CardResponse>(mapped);
    }

    public async Task<ApiResponse<List<CardResponse>>> Handle(GetCardByCustomerIdQuery request,
        CancellationToken cancellationToken)
    {
        List<Card> list = await dbContext.Set<Card>()
            .Include(x => x.Account)
            .Where(x => x.Account.CustomerId == request.CustomerId)
            .ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<CardResponse>>(list);
        return new ApiResponse<List<CardResponse>>(mapped);
    }

    public async Task<ApiResponse<CardResponse>> Handle(GetCardByAccountIdQuery request,
        CancellationToken cancellationToken)
    {
        Card? entity = await dbContext.Set<Card>()
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.AccountId == request.AccountId, cancellationToken);

        CardResponse mapped = mapper.Map<CardResponse>(entity);
        return new ApiResponse<CardResponse>(mapped);
    }
}