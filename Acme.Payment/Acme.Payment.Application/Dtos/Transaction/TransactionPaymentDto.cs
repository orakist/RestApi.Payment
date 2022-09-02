using System.ComponentModel.DataAnnotations;
using Acme.Payment.Domain.Utility;

namespace Acme.Payment.Application.Dtos;

public class TransactionPaymentDto : IValidatableObject
{
    [Required]
    public string MessageType { get; set; }

    [Required]
    public Guid TransactionId { get; set; }

    [Required]
    public long AccountNumber { get; set; }

    [Required]
    public string Origin { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        MessageType = SharedRegex.ClearSingleLineText(MessageType);
        if (MessageType != "PAYMENT" && MessageType != "ADJUSTMENT")
            yield return new ValidationResult($"Message Type is Invalid! MessageType: {MessageType}");

        Origin = SharedRegex.ClearSingleLineText(Origin);
        if (Origin != "VISA" && Origin != "MASTER")
            yield return new ValidationResult($"Transaction Origin is Invalid! Origin: {Amount}");

        if (Amount <= 0 || Amount > 100000) 
            yield return new ValidationResult($"Amount is invalid! Amount: {Amount}");
    }
}