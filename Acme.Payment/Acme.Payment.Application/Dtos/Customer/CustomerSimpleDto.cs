using Acme.Foundation.Application.Dtos;

namespace Acme.Payment.Application.Dtos;

public class CustomerSimpleDto : EntityDto<Guid>
{
    public string Name { get; set; }
}