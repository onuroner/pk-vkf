using Microsoft.AspNetCore.Mvc;
using Vk.Data.Domain;
using Vk.Data.Uow;

namespace VkApi.Controllers;


[NonController]
[Route("vk/api/v1/[controller]")]
[ApiController]
public class Customers2Controller : ControllerBase
{
    private readonly IUnitOfWork unitOfWork;
    public Customers2Controller(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
    
    
    [HttpGet]
    public List<Customer> Get()
    {
        return unitOfWork.CustomerRepository.GetAll();
    }

    [HttpGet("{id}")]
    public Customer Get(int id)
    {
        return unitOfWork.CustomerRepository.GetById(id);
    }

    [HttpPost]
    public void Post([FromBody] Customer request)
    {
        unitOfWork.CustomerRepository.Insert(request);
        unitOfWork.Complete();
    }
    
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] Customer request)
    {
        unitOfWork.CustomerRepository.Update(request);
        unitOfWork.Complete();
    }
    
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
        unitOfWork.CustomerRepository.Delete(id);
        unitOfWork.Complete();
    }
    
    
}