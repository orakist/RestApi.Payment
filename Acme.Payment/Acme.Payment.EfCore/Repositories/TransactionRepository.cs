using Acme.Payment.Domain.Entities;
using Acme.Payment.Domain.Repositories;
using Acme.Payment.EfCore.DbContext;

namespace Acme.Payment.EfCore.Repositories;

public class TransactionRepository : EfCoreRepository<Transaction, Guid>, ITransactionRepository
{
    public TransactionRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) 
        : base(dbContext)
    {
    }
}
