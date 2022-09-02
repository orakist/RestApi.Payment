using Acme.Foundation.Application.Dtos;
using Acme.Foundation.Application.Services;
using Acme.Payment.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Payment.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountAppService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger, IAccountAppService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<AccountDto> GetAsync(Guid id)
        {
            
            return _accountService.GetAsync(id);
        }

        [HttpGet]
        [Route("list")]
        public Task<PagedResultDto<AccountSimpleDto>> GetListAsync([FromQuery] AccountGetListDto input)
        {
            return _accountService.GetListAsync(input);
        }

        [HttpPost]
        [Route("create")]
        public Task<AccountDto> CreateAsync(AccountCreateDto input)
        {
            return _accountService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}/update")]
        public Task<AccountDto> UpdateAsync(Guid id, AccountUpdateDto input)
        {
            return _accountService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}/delete")]
        public Task DeleteAsync(Guid id)
        {
            return _accountService.DeleteAsync(id);
        }
    }
}