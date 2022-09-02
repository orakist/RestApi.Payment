using Acme.Foundation.Application.Dtos;

namespace Acme.Payment.Application.Dtos;

public class AccountDto : AuditedEntityDto<Guid>
{
    public long AccountNumber { get; set; }

    public string AccountName { get; set; }

    public decimal Balance { get; set; }

    public CustomerSimpleDto Owner { get; set; }

    public List<TransactionSimpleDto> Transactions { get; set; }
}