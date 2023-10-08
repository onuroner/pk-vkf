using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vk.Data.Context;
using Vk.Data.Domain;
using Vk.Data.Uow;

namespace Vk.Api.Controllers;


[NonController]
[Route("vk/api/v1/[controller]")]
[ApiController]
public class CustomersOldController : ControllerBase
{
    private readonly VkDbContext dbContext;
    private readonly IUnitOfWork unitOfWork;
    public CustomersOldController(VkDbContext dbContext,IUnitOfWork unitOfWork)
    {
        this.dbContext = dbContext;
        this.unitOfWork = unitOfWork;
    }


    [HttpGet]
    public void Get()
    {
        var list01 = unitOfWork.CustomerRepository.GetAll();
        var list02 = dbContext.Set<Customer>().AsNoTracking().ToList();
        var list03 = dbContext.Customers.AsNoTracking().ToList();

        var list2 = dbContext.Set<Customer>().Include(x=> x.Addresses).ToList();

        var list3 = dbContext.Set<Customer>().Include(x=> x.Accounts).ToList();

        var list4 = dbContext.Set<Customer>().Include(x=> x.Accounts).Include(x=> x.Addresses).ToList();

        var list5 = dbContext.Set<Customer>()
            .Include(x => x.Accounts).ThenInclude(x=> x.AccountTransactions)
            .Include(x => x.Accounts).ThenInclude(x => x.EftTransactions)
            .Include(x => x.Accounts).ThenInclude(x => x.Card)
            .Include(x => x.Addresses).ToList();


    }


}
