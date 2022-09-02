using System.ComponentModel.DataAnnotations;
using Acme.Foundation.Application.Dtos;
using Acme.Payment.Application.Dtos;
using Acme.Payment.Domain.Repositories;
using System.Linq.Dynamic.Core;
using Acme.Payment.Domain.Entities;
using AutoMapper;

namespace Acme.Foundation.Application.Services;

public class AccountAppService : IAccountAppService
{
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepo;
    private readonly ITransactionRepository _transactionRepo;
    private readonly ICustomerRepository _customerRepo;

    public AccountAppService(
        IMapper mapper, IAccountRepository accountRepo, 
        ITransactionRepository transactionRepo, ICustomerRepository customerRepo)
    {
        _accountRepo = accountRepo;
        _mapper = mapper;
        _transactionRepo = transactionRepo;
        _customerRepo = customerRepo;
    }

    public async Task<AccountDto> GetAsync(Guid id)
    {
        var account = await _accountRepo.GetAsync(id);
        var customer = await _customerRepo.GetAsync(account.OwnerId);
        var transactions = await _transactionRepo.GetListAsync(a => a.AccountId == account.Id);

        var result = _mapper.Map<Account, AccountDto>(account);
        result.Owner = _mapper.Map<Customer, CustomerSimpleDto>(customer);
        result.Transactions = _mapper.Map<List<Transaction>, List<TransactionSimpleDto>>(transactions);
        return result;
    }

    public async Task<PagedResultDto<AccountSimpleDto>> GetListAsync(AccountGetListDto input)
    {
        var query = (await _accountRepo.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.FilterText), a => a.AccountName.Contains(input.FilterText));
        var totalCount = query.Count();

        var entities = query.OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "Id" : input.Sorting)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

        return new PagedResultDto<AccountSimpleDto>
        {
            TotalCount = totalCount,
            Items = _mapper.Map<List<Account>, List<AccountSimpleDto>>(entities)
        };
    }

    public async Task<AccountDto> CreateAsync(AccountCreateDto input)
    {
        var account = new Account(
            Guid.NewGuid(), input.AccountNumber,
            input.AccountName, input.Balance ?? 0, input.OwnerId);

        account = await _accountRepo.InsertAsync(account, true);
        var customer = await _customerRepo.GetAsync(account.OwnerId);

        var result = _mapper.Map<Account, AccountDto>(account);
        result.Owner = _mapper.Map<Customer, CustomerSimpleDto>(customer);
        return result;
    }

    public async Task<AccountDto> UpdateAsync(Guid id, AccountUpdateDto input)
    {
        var account = await _accountRepo.GetAsync(id);
        account.AccountName = input.AccountName;
        if (input.Balance.HasValue)
            account.Balance = input.Balance.Value;

        account = await _accountRepo.UpdateAsync(account, true);
        var customer = await _customerRepo.GetAsync(account.OwnerId);

        var result = _mapper.Map<Account, AccountDto>(account);
        result.Owner = _mapper.Map<Customer, CustomerSimpleDto>(customer);
        return result;
    }

    public async Task DeleteAsync(Guid id)
    {
        var account = await _accountRepo.GetAsync(id);
        if (account.Balance > 0)
            throw new ValidationException("Account balance must be zero!");

        await _accountRepo.DeleteAsync(account, true);
    }
}