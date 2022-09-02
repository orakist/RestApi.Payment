using Acme.Foundation.Domain.Repositories;
using Acme.Payment.Domain.Entities;

namespace Acme.Payment.Domain.Repositories;

public interface ICustomerRepository : IRepository<Customer, Guid>
{
}
