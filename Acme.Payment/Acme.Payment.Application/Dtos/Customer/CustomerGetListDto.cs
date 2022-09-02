using Acme.Foundation.Application.Dtos;

namespace Acme.Payment.Application.Dtos;

public class CustomerGetListDto : PagedAndSortedResultRequestDto
{
    public string FilterText { get; set; }
}