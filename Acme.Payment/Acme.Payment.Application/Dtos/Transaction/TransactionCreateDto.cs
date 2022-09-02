using System.ComponentModel.DataAnnotations;
using Acme.Payment.Domain.Utility;

namespace Acme.Payment.Application.Dtos;

public class TransactionCreateDto : IValidatableObject
{
    [Required]
    public long AccountNumber { get; set; }

    [Required]
    public string Origin { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        Origin = SharedRegex.ClearSingleLineText(Origin);
        if (Origin != "VISA" && Origin != "MASTER")
            yield return new ValidationResult($"Transaction Origin is Invalid! Origin: {Amount}");

        if (Amount <= 0 || Amount > 100000) 
            yield return new ValidationResult($"Amount is invalid! Amount: {Amount}");
    }
}