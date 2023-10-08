using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vk.Base.Response;
using Vk.Data.Context;
using Vk.Data.Domain;
using Vk.Schema;

namespace Vk.Operation;

public class EftTransactionQueryHandler :
    IRequestHandler<GetAllEftTransactionQuery, ApiResponse<List<EftTransactionResponse>>>,
    IRequestHandler<GetEftTransactionByIdQuery, ApiResponse<EftTransactionResponse>>
{
    private readonly VkDbContext dbContext;
    private readonly IMapper mapper;

    public EftTransactionQueryHandler(VkDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }


    public async Task<ApiResponse<List<EftTransactionResponse>>> Handle(GetAllEftTransactionQuery request,
        CancellationToken cancellationToken)
    {
        List<EftTransaction> list = await dbContext.Set<EftTransaction>()
            .Include(x => x.Account)
            .ToListAsync(cancellationToken);
        
        List<EftTransactionResponse> mapped = mapper.Map<List<EftTransactionResponse>>(list);
        return new ApiResponse<List<EftTransactionResponse>>(mapped);
    }

    public async Task<ApiResponse<EftTransactionResponse>> Handle(GetEftTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        EftTransaction? entity = await dbContext.Set<EftTransaction>()
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == request.Id,cancellationToken);
        
        if (entity == null)
        {
            return new ApiResponse<EftTransactionResponse>("Record not found!");
        }
        
        EftTransactionResponse mapped = mapper.Map<EftTransactionResponse>(entity);
        return new ApiResponse<EftTransactionResponse>(mapped);
    }
}