using Acme.Payment.Application.Dtos;

namespace Acme.Foundation.Application.Services;

public interface ICustomerAppService :
    ICrudAppService<CustomerDto, CustomerSimpleDto, Guid, CustomerGetListDto, CustomerCreateDto, CustomerUpdateDto>
{
}
