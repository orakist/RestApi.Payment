using System.ComponentModel.DataAnnotations;
using Acme.Payment.Domain.Utility;

namespace Acme.Payment.Application.Dtos;

public class CustomerCreateDto : IValidatableObject
{
    [Required]
    [StringLength(64)]
    public string Name { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        Name = SharedRegex.ClearSingleLineText(Name);
        yield break;
    }
}