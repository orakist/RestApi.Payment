using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using Acme.Foundation.Application.Dtos;
using Acme.Payment.Application.Dtos;
using Acme.Payment.Domain.Entities;
using Acme.Payment.Domain.Repositories;
using AutoMapper;

namespace Acme.Foundation.Application.Services;

public class CustomerAppService : ICustomerAppService
{
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepo;
    private readonly ICustomerRepository _customerRepo;

    public CustomerAppService(
        IAccountRepository accountRepo, IMapper mapper, ICustomerRepository customerRepo)
    {
        _customerRepo = customerRepo;
        _accountRepo = accountRepo;
        _mapper = mapper;
    }

    public async Task<CustomerDto> GetAsync(Guid id)
    {
        var customer = await _customerRepo.GetAsync(id);
        var accounts = await _accountRepo.GetListAsync(a => a.OwnerId == customer.Id);

        var result = _mapper.Map<Customer, CustomerDto>(customer);
        result.Accounts = _mapper.Map<List<Account>, List<AccountSimpleDto>>(accounts);

        return result;
    }

    public async Task<PagedResultDto<CustomerSimpleDto>> GetListAsync(CustomerGetListDto input)
    {
        var query = (await _customerRepo.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.FilterText), a => a.Name.Contains(input.FilterText));
        var totalCount = query.Count();

        var entities = query.OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "Id" : input.Sorting)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

        return new PagedResultDto<CustomerSimpleDto>
        {
            TotalCount = totalCount,
            Items = _mapper.Map<List<Customer>, List<CustomerSimpleDto>>(entities)
        };
    }

    public async Task<CustomerDto> CreateAsync(CustomerCreateDto input)
    {
        var customer = new Customer(Guid.NewGuid(), input.Name);
        customer = await _customerRepo.InsertAsync(customer, true);

        return _mapper.Map<Customer, CustomerDto>(customer);
    }

    public async Task<CustomerDto> UpdateAsync(Guid id, CustomerUpdateDto input)
    {
        var customer = await _customerRepo.GetAsync(id);
        customer.Name = input.Name;
        customer = await _customerRepo.UpdateAsync(customer, true);

        return _mapper.Map<Customer, CustomerDto>(customer);
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await _customerRepo.GetAsync(id);

        var accountCount = await _accountRepo.CountAsync(a => a.OwnerId == customer.Id);
        if (accountCount > 0)
            throw new ValidationException("Customer has active account(s). The customer can't be deleted!");

        await _customerRepo.DeleteAsync(customer, true);
    }
}