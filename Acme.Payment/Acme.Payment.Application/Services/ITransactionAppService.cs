using Acme.Payment.Application.Dtos;

namespace Acme.Foundation.Application.Services;

public interface ITransactionAppService :
    ICrudAppService<TransactionDto, TransactionSimpleDto, Guid, TransactionGetListDto, TransactionCreateDto, TransactionUpdateDto>
{
    Task<TransactionDto> PaymentAsync(TransactionPaymentDto input);
}
