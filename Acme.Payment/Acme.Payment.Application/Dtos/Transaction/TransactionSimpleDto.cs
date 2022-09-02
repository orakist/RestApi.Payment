using Acme.Foundation.Application.Dtos;

namespace Acme.Payment.Application.Dtos;

public class TransactionSimpleDto : EntityDto<Guid>
{
    public string MessageType { get; set; }

    public long AccountNumber { get; set; }

    public string Origin { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; }
}