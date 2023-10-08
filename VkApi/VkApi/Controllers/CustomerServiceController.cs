using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vk.Base.Response;
using Vk.Operation;
using Vk.Schema;

namespace VkApi.Controllers;


[Route("vk/api/v1/[controller]")]
[ApiController]
public class CustomerServiceController: ControllerBase
{
    private IMediator mediator;

    public CustomerServiceController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    
    [HttpGet("GetCustomerAccounts")]
    [Authorize(Roles = "customer")]
    public async Task<ApiResponse<List<AccountResponse>>> GetCustomerAccounts()
    {
        var id = (User.Identity as ClaimsIdentity).FindFirst("Id").Value;
        var operation = new GetAccountByCustomerIdQuery(int.Parse(id));
        var result = await mediator.Send(operation);
        return result;
    }

      
    [HttpGet("CustomerInfo")]
    [Authorize(Roles = "customer")]
    public async Task<ApiResponse<CustomerResponse>> CustomerInfo()
    {
        var id = (User.Identity as ClaimsIdentity).FindFirst("Id").Value;
        var operation = new GetCustomerByIdQuery(int.Parse(id));
        var result = await mediator.Send(operation);
        return result;
    }

}