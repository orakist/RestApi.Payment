using Acme.Foundation.Application.Dtos;

namespace Acme.Payment.Application.Dtos;

public class TransactionDto : AuditedEntityDto<Guid>
{
    public string MessageType { get; set; }

    public string Origin { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; }

    public AccountSimpleDto Account { get; set; }

    public List<AdjustmentDto> Adjustments { get; set; }
}