using Acme.Payment.Domain.Entities;
using Acme.Payment.Domain.Repositories;
using Acme.Payment.EfCore.DbContext;

namespace Acme.Payment.EfCore.Repositories;

public class AdjustmentRepository : EfCoreRepository<Adjustment, Guid>, IAdjustmentRepository
{
    public AdjustmentRepository(Microsoft.EntityFrameworkCore.DbContext dbContext) 
        : base(dbContext)
    {
    }
}
