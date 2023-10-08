using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vk.Base.Response;
using Vk.Operation;
using Vk.Schema;

namespace VkApi.Controllers;

[Route("vk/api/v1/[controller]")]
[ApiController]
public class CardsController : ControllerBase
{
    private IMediator mediator;

    public CardsController(IMediator mediator)
    {
        this.mediator = mediator;
    }


    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ApiResponse<List<CardResponse>>> GetAll()
    {
        var operation = new GetAllCardQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ApiResponse<CardResponse>> Get(int id)
    {
        var operation = new GetCardByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }
    
    [HttpGet("ByCustomerId/{customerid}")]
    [Authorize(Roles = "admin")]
    public async Task<ApiResponse<List<CardResponse>>> GetByCustomerId(int customerid)
    {
        var operation = new GetCardByCustomerIdQuery(customerid);
        var result = await mediator.Send(operation);
        return result;
    }
    
    [HttpGet("ByAccountId/{accountid}")]
    [Authorize(Roles = "admin")]
    public async Task<ApiResponse<CardResponse>> GetByAccountId(int accountid)
    {
        var operation = new GetCardByAccountIdQuery(accountid);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ApiResponse<CardResponse>> Post([FromBody] CardRequest request)
    {
        var operation = new CreateCardCommand(request);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ApiResponse> Put(int id, [FromBody] CardRequest request)
    {
        var operation = new UpdateCardCommand(request, id);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ApiResponse> Delete(int id)
    {
        var operation = new DeleteCardCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}