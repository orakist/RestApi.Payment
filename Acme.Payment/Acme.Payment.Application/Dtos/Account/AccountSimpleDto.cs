using Acme.Foundation.Application.Dtos;

namespace Acme.Payment.Application.Dtos;

public class AccountSimpleDto : EntityDto<Guid>
{
    public long AccountNumber { get; set; }

    public string AccountName { get; set; }

    public decimal Balance { get; set; }

    public Guid OwnerId { get; set; }
}