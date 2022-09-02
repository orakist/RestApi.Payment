using Acme.Foundation.Domain.Entities;

namespace Acme.Payment.Domain.Entities;

public class Transaction : FullAuditedEntity<Guid>
{
    public string MessageType { get; set; }

    public Guid AccountId { get; set; }

    public string Origin { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; private set; }

    public Transaction(Guid id, string messageType, Guid accountId, string origin, decimal amount)
        : base(id)
    {
        MessageType = messageType;
        AccountId = accountId;
        Origin = origin;
        Amount = amount;
    }

    public decimal CalculateBalance(decimal balance)
    {
        var commission = Origin == "VISA" ? Amount * 0.01m : Amount * 0.02m;
        var finalBalance = balance - commission - Amount;

        string text1 = $"Commission: {Amount:F2} * {(Origin == "VISA" ? "0.01" : "0.02")} = {commission:F2}";
        string text2 = $"Final Balance: {balance:F2} - {Amount:F2} - {commission:F2} = {finalBalance:F2}";
        Description = $"Origin: {Origin}, {text1}, {text2}";

        return finalBalance;
    }
}
