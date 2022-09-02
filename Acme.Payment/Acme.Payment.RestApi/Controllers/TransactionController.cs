using Acme.Foundation.Application.Dtos;
using Acme.Foundation.Application.Services;
using Acme.Payment.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Payment.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionAppService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ILogger<TransactionController> logger, ITransactionAppService transactionService)
        {
            _logger = logger;
            _transactionService = transactionService;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<TransactionDto> GetAsync(Guid id)
        {
            return _transactionService.GetAsync(id);
        }

        [HttpGet]
        [Route("list")]
        public Task<PagedResultDto<TransactionSimpleDto>> GetListAsync([FromQuery] TransactionGetListDto input)
        {
            return _transactionService.GetListAsync(input);
        }

        [HttpPost]
        [Route("create")]
        public Task<TransactionDto> CreateAsync(TransactionCreateDto input)
        {
            return _transactionService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}/update")]
        public Task<TransactionDto> UpdateAsync(Guid id, TransactionUpdateDto input)
        {
            return _transactionService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}/delete")]
        public Task DeleteAsync(Guid id)
        {
            return _transactionService.DeleteAsync(id);
        }
        
        [HttpPost]
        [Route("payment")]
        public Task<TransactionDto> PaymentAsync(TransactionPaymentDto input)
        {
            return _transactionService.PaymentAsync(input);
        }
    }
}