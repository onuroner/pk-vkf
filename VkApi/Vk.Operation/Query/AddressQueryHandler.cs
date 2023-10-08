using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vk.Base.Response;
using Vk.Data.Context;
using Vk.Data.Domain;
using Vk.Schema;

namespace Vk.Operation;

public class AddressQueryHandler :
    IRequestHandler<GetAllAddressQuery, ApiResponse<List<AddressResponse>>>,
    IRequestHandler<GetAddressByIdQuery, ApiResponse<AddressResponse>>,
    IRequestHandler<GetAddressByCustomerIdQuery , ApiResponse<List<AddressResponse>>>
{
    private readonly VkDbContext dbContext;
    private readonly IMapper mapper;

    public AddressQueryHandler(VkDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }


    public async Task<ApiResponse<List<AddressResponse>>> Handle(GetAllAddressQuery request,
        CancellationToken cancellationToken)
    {
        List<Address> list = await dbContext.Set<Address>()
            .Include(x => x.Customer)
            .ToListAsync(cancellationToken);
        
        List<AddressResponse> mapped = mapper.Map<List<AddressResponse>>(list);
        return new ApiResponse<List<AddressResponse>>(mapped);
    }

    public async Task<ApiResponse<AddressResponse>> Handle(GetAddressByIdQuery request,
        CancellationToken cancellationToken)
    {
        Address? entity = await dbContext.Set<Address>()
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.Id,cancellationToken);
        
        if (entity == null)
        {
            return new ApiResponse<AddressResponse>("Record not found!");
        }
        
        AddressResponse mapped = mapper.Map<AddressResponse>(entity);
        return new ApiResponse<AddressResponse>(mapped);
    }

    public async Task<ApiResponse<List<AddressResponse>>> Handle(GetAddressByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        List<Address> list = await dbContext.Set<Address>()
            .Include(x => x.Customer)
            .Where(x => x.CustomerId == request.CustomerId)
            .ToListAsync(cancellationToken);

        var mapped = mapper.Map<List<AddressResponse>>(list);
        return new ApiResponse<List<AddressResponse>>(mapped);
    }
}