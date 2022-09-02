using Acme.Payment.Application.Dtos;

namespace Acme.Foundation.Application.Services;

public interface IAccountAppService :
    ICrudAppService<AccountDto, AccountSimpleDto, Guid, AccountGetListDto, AccountCreateDto, AccountUpdateDto>
{
}
