using Acme.Foundation.Application.Dtos;

namespace Acme.Payment.Application.Dtos;

public class AccountGetListDto : PagedAndSortedResultRequestDto
{
    public string FilterText { get; set; }
}