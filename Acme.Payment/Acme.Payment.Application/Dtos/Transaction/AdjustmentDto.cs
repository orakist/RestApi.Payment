using Acme.Foundation.Application.Dtos;

namespace Acme.Payment.Application.Dtos;

public class AdjustmentDto : CreationAuditedEntityDto<Guid>
{
    public string OldOrigin { get; set; }

    public string NewOrigin { get; set; }

    public decimal OldAmount { get; set; }

    public decimal NewAmount { get; set; }
}