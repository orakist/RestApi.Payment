using System.ComponentModel.DataAnnotations;
using Acme.Payment.Domain.Utility;

namespace Acme.Payment.Application.Dtos;

public class AccountCreateDto : IValidatableObject
{
    [Required]
    public long AccountNumber { get; set; }

    [Required]
    [StringLength(64)]
    public string AccountName { get; set; }

    public decimal? Balance { get; set; }

    [Required]
    public Guid OwnerId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        AccountName = SharedRegex.ClearSingleLineText(AccountName);

        if (Balance <= 0 || Balance > 100000)
            yield return new ValidationResult($"Balance is invalid! Balance: {Balance}");
    }
}