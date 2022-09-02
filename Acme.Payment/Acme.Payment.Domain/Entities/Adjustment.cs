using Acme.Foundation.Domain.Entities;

namespace Acme.Payment.Domain.Entities;

public class Adjustment : CreationAuditedEntity<Guid>
{
    public Guid TransactionId { get; set; }

    public string OldOrigin { get; set; }

    public string NewOrigin { get; set; }

    public decimal OldAmount { get; set; }

    public decimal NewAmount { get; set; }

    public Adjustment(Guid id, Guid transactionId, decimal oldAmount, decimal newAmount, string oldOrigin, string newOrigin)
        : base(id)
    {
        TransactionId = transactionId;
        OldAmount = oldAmount;
        NewAmount = newAmount;
        OldOrigin = oldOrigin;
        NewOrigin = newOrigin;
    }
}
