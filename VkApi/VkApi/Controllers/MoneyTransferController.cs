using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Vk.Base.Response;
using Vk.Operation;
using Vk.Schema;

namespace VkApi.Controllers;

[Route("vk/api/v1/[controller]")]
[ApiController]
public class MoneyTransferController : ControllerBase
{
    private IMediator mediator;
    private readonly IMemoryCache memoryCache;
    private readonly IDistributedCache distributedCache;

    public MoneyTransferController(IMediator mediator, IMemoryCache memoryCache, IDistributedCache distributedCache)
    {
        this.mediator = mediator;
        this.memoryCache = memoryCache;
        this.distributedCache = distributedCache;
    }


    [HttpPost("InternalTransfer")]
    public async Task<ApiResponse<MoneyTransferResponse>> Post([FromBody] MoneyTransferRequest request)
    {
        var operation = new CreateMoneyTransferCommand(request);
        var result = await mediator.Send(operation);
        return result;
    }


    [HttpGet("ByReferenceNumber/{referenceNumber}")]
    public async Task<ApiResponse<List<AccountTransactionResponse>>> ByReferenceNumber(string referenceNumber)
    {
        var cacheResult =
            memoryCache.TryGetValue(referenceNumber, out ApiResponse<List<AccountTransactionResponse>> cacheData);
        if (cacheResult)
        {
            return cacheData;
        }

        var operation = new GetMoneyTransferByReferenceQuery(referenceNumber);
        var result = await mediator.Send(operation);

        if (result.Response.Any())
        {
            var cacheOptions = new MemoryCacheEntryOptions()
            {
                Priority = CacheItemPriority.Low,
                AbsoluteExpiration = DateTime.Now.AddDays(1),
                SlidingExpiration = TimeSpan.FromMinutes(60)
            };
            memoryCache.Set(referenceNumber, result, cacheOptions);
        }

        return result;
    }


    [HttpGet("ByReference/{referenceNumber}")]
    public async Task<ApiResponse<List<AccountTransactionResponse>>> ByReference(string referenceNumber)
    {
        var cacheResult = await distributedCache.GetAsync(referenceNumber);
        if (cacheResult != null)
        {
            string json = Encoding.UTF8.GetString(cacheResult);
            var response = JsonConvert.DeserializeObject<List<AccountTransactionResponse>>(json);
            return new ApiResponse<List<AccountTransactionResponse>>(response);
        }

        var operation = new GetMoneyTransferByReferenceQuery(referenceNumber);
        var result = await mediator.Send(operation);

        if (result.Response.Any())
        {
            string responsejson = JsonConvert.SerializeObject(result.Response);
            byte[] responseArr = Encoding.UTF8.GetBytes(responsejson);

            var cacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddDays(1),
                SlidingExpiration = TimeSpan.FromMinutes(60)
            };
            await distributedCache.SetAsync(referenceNumber, responseArr, cacheOptions);
        }

        return result;
    }

    [HttpGet("ByAccountId/{accountId}")]
    public async Task<ApiResponse<List<AccountTransactionResponse>>> Post(int accountId)
    {
        var operation = new GetMoneyTransferByAccountIdQuery(accountId);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("ByParameter")]
    //[ResponseCache(CacheProfileName = "Cache100")]
    [ResponseCache(Duration = 100, Location = ResponseCacheLocation.Any)]
    public async Task<ApiResponse<List<AccountTransactionResponse>>> ByParameter(
        [FromQuery] int? accountId,
        [FromQuery] int? customerId,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount,
        [FromQuery] DateTime? beginDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? description = null
    )
    {
        var operation = new GetMoneyTransferByParametersQuery(accountId, customerId, minAmount,
            maxAmount, beginDate, endDate, description);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("RemoveCache")]
    public ApiResponse RemoveCache([FromQuery] string cacheKey)
    {
        memoryCache.Remove(cacheKey);
        distributedCache.Remove(cacheKey);
        return new ApiResponse();
    }
}