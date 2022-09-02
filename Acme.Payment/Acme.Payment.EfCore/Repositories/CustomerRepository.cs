using Acme.Payment.Domain.Entities;
using Acme.Payment.Domain.Repositories;
using Acme.Payment.EfCore.DbContext;

namespace Acme.Payment.EfCore.Repositories;

public class CustomerRepository : EfCoreRepository<Customer, Guid>, ICustomerRepository
{
    public CustomerRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) 
        : base(dbContext)
    {
    }
}
