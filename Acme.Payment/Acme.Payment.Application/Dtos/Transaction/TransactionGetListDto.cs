using Acme.Foundation.Application.Dtos;

namespace Acme.Payment.Application.Dtos;

public class TransactionGetListDto : PagedAndSortedResultRequestDto
{
    public long AccountNumber { get; set; }
}