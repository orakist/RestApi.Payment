using Acme.Foundation.Application.Dtos;

namespace Acme.Payment.Application.Dtos;

public class CustomerDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }

    public List<AccountSimpleDto> Accounts { get; set; }
}