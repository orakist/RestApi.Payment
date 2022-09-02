using System.ComponentModel.DataAnnotations;
using Acme.Foundation.Application.Dtos;
using Acme.Payment.Application.Dtos;
using Acme.Payment.Domain.Entities;
using Acme.Payment.Domain.Repositories;
using System.Linq.Dynamic.Core;
using AutoMapper;

namespace Acme.Foundation.Application.Services;

public class TransactionAppService : ITransactionAppService
{
    private readonly IMapper _mapper;
    private readonly IAccountRepository _accountRepo;
    private readonly IAdjustmentRepository _adjustmentRepo;
    private readonly ITransactionRepository _transactionRepo;

    public TransactionAppService(
        ITransactionRepository transactionRepo, IMapper mapper,
        IAccountRepository accountRepo, IAdjustmentRepository adjustmentRepo)
    {
        _transactionRepo = transactionRepo;
        _mapper = mapper;
        _accountRepo = accountRepo;
        _adjustmentRepo = adjustmentRepo;
    }

    public async Task<TransactionDto> GetAsync(Guid id)
    {
        var transaction = await _transactionRepo.GetAsync(id);
        var account = await _accountRepo.GetAsync(transaction.AccountId);
        var adjustments = await _adjustmentRepo.GetListAsync(a => a.TransactionId == transaction.Id);

        var result = _mapper.Map<Transaction, TransactionDto>(transaction);
        result.Account = _mapper.Map<Account, AccountSimpleDto>(account);
        result.Adjustments = _mapper.Map<List<Adjustment>, List<AdjustmentDto>>(adjustments);

        return result;
    }

    public async Task<PagedResultDto<TransactionSimpleDto>> GetListAsync(TransactionGetListDto input)
    {
        var accounts = input.AccountNumber > 0
            ? await _accountRepo.GetListAsync()
            : await _accountRepo.GetListAsync(a => a.AccountNumber == input.AccountNumber);

        var accountIds = accounts.Select(a => a.Id);
        var query = (await _transactionRepo.GetQueryableAsync()).Where(a => accountIds.Contains(a.AccountId));
        var totalCount = query.Count();

        var entities = query.OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "Id" : input.Sorting)
            .Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

        var items = entities.Select(transaction =>
        {
            var item = _mapper.Map<Transaction, TransactionSimpleDto>(transaction);
            item.AccountNumber = accounts.FirstOrDefault(s => s.Id == transaction.AccountId)?.AccountNumber ?? 0;
            return item;
        }).ToList();

        return new PagedResultDto<TransactionSimpleDto> { TotalCount = totalCount, Items = items };
    }

    public async Task<TransactionDto> CreateAsync(TransactionCreateDto input)
    {
        return await CreateTransaction(input.AccountNumber, input.Origin, input.Amount);
    }

    public async Task<TransactionDto> UpdateAsync(Guid id, TransactionUpdateDto input)
    {
        return await UpdateTransaction(id, input.Origin, input.Amount);
    }

    public async Task<TransactionDto> PaymentAsync(TransactionPaymentDto input)
    {
        if (input.MessageType == "PAYMENT")
            return await CreateTransaction(input.AccountNumber, input.Origin, input.Amount);

        return await UpdateTransaction(input.TransactionId, input.Origin, input.Amount);
    }

    public async Task DeleteAsync(Guid id)
    {
        var transaction = await _transactionRepo.GetAsync(id);

        if (transaction.Amount > 0)
            throw new ValidationException("Transaction amount is bigger than zero, transaction cannot be deleted!");

        await _transactionRepo.DeleteAsync(transaction, true);
    }

    private async Task<TransactionDto> CreateTransaction(long accountNumber, string origin, decimal amount)
    {
        var account = await _accountRepo.FindAsync(a => a.AccountNumber == accountNumber);
        if (account == null)
            throw new ValidationException("Account Number not found!");
        if (amount < 1)
            throw new ValidationException("Transaction amount must be at least 1.00.");

        // Calculate commission and validate balance
        var commission = origin == "VISA" ? amount * 0.01m : amount * 0.02m;
        if (account.Balance < commission + amount)
            throw new ValidationException("Account balance is not enough for this transaction.");

        var transaction = new Transaction(
            Guid.NewGuid(), "PAYMENT", account.Id, origin, amount);
        account.Balance = transaction.CalculateBalance(account.Balance);

        // Update the database
        transaction = await _transactionRepo.InsertAsync(transaction, true);
        await _accountRepo.UpdateAsync(account, true);

        var result = _mapper.Map<Transaction, TransactionDto>(transaction);
        result.Account = _mapper.Map<Account, AccountSimpleDto>(account);
        return result;
    }

    private async Task<TransactionDto> UpdateTransaction(Guid id, string origin, decimal amount)
    {
        var transaction = await _transactionRepo.GetAsync(id);
        var account = await _accountRepo.GetAsync(transaction.AccountId);

        var originalCommission = transaction.Origin == "VISA" ? transaction.Amount * 0.01m : transaction.Amount * 0.02m;
        var originalBalance = account.Balance + transaction.Amount + originalCommission;

        // Calculate commission and validate balance
        var commission = origin == "VISA" ? amount * 0.01m : amount * 0.02m;
        if (originalBalance < commission + amount)
            throw new ValidationException("Account balance is not enough for this transaction.");

        var adjustment = new Adjustment(
            Guid.NewGuid(), transaction.Id, transaction.Amount, amount, transaction.Origin, origin);

        transaction.Amount = amount;
        transaction.Origin = origin;
        account.Balance = transaction.CalculateBalance(originalBalance);

        // Update the database
        transaction = await _transactionRepo.UpdateAsync(transaction, true);
        await _accountRepo.UpdateAsync(account, true);
        await _adjustmentRepo.InsertAsync(adjustment, true);

        var adjustments = await _adjustmentRepo.GetListAsync(a => a.TransactionId == transaction.Id);
        var result = _mapper.Map<Transaction, TransactionDto>(transaction);
        result.Account = _mapper.Map<Account, AccountSimpleDto>(account);
        result.Adjustments = _mapper.Map<List<Adjustment>, List<AdjustmentDto>>(adjustments);

        return result;
    }
}