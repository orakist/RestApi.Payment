using Acme.Foundation.Domain.Entities;

namespace Acme.Payment.Domain.Entities;

public class Account : FullAuditedEntity<Guid>
{
    public long AccountNumber { get; set; }

    public string AccountName { get; set; }

    public decimal Balance { get; set; }
    
    public Guid OwnerId { get; set; }

    public Account(Guid id, long accountNumber, string accountName, decimal balance, Guid ownerId)
        : base(id)
    {
        AccountNumber = accountNumber;
        AccountName = accountName;
        Balance = balance;
        OwnerId = ownerId;
    }
}
