using Acme.Payment.Domain.Entities;
using Acme.Payment.Domain.Repositories;
using Acme.Payment.EfCore.DbContext;

namespace Acme.Payment.EfCore.Repositories;

public class AccountRepository : EfCoreRepository<Account, Guid>, IAccountRepository
{
    public AccountRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) 
        : base(dbContext)
    {
    }
}
