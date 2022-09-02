using Acme.Foundation.Application.Dtos;
using Acme.Foundation.Application.Services;
using Acme.Payment.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Payment.RestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerAppService _customerService;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ILogger<CustomerController> logger, ICustomerAppService customerService)
    {
        _logger = logger;
        _customerService = customerService;
    }

    [HttpGet]
    [Route("{id}")]
    public Task<CustomerDto> GetAsync(Guid id)
    {
        return _customerService.GetAsync(id);
    }

    [HttpGet]
    [Route("list")]
    public Task<PagedResultDto<CustomerSimpleDto>> GetListAsync([FromQuery] CustomerGetListDto input)
    {
        return _customerService.GetListAsync(input);
    }

    [HttpPost]
    [Route("create")]
    public Task<CustomerDto> CreateAsync(CustomerCreateDto input)
    {
        return _customerService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}/update")]
    public Task<CustomerDto> UpdateAsync(Guid id, CustomerUpdateDto input)
    {
        return _customerService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}/delete")]
    public Task DeleteAsync(Guid id)
    {
        return _customerService.DeleteAsync(id);
    }
}