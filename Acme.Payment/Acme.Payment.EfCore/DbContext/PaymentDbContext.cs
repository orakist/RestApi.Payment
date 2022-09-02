using Acme.Foundation.Domain.Auditing;
using Acme.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Acme.Payment.EfCore.DbContext;

public class PaymentDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Account> Accounts { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Adjustment> Adjustments { get; set; }


    private readonly IAuditPropertySetter _propertySetter;

    public PaymentDbContext(
        DbContextOptions<PaymentDbContext> options, 
        IAuditPropertySetter propertySetter)
        : base(options)
    {
        _propertySetter = propertySetter;
        ChangeTracker.Tracked += ChangeTracker_Tracked;
    }

    private void ChangeTracker_Tracked(object sender, EntityTrackedEventArgs e)
    {
        switch (e.Entry.State)
        {
            case EntityState.Added:
                _propertySetter.SetIdProperty(e.Entry.Entity);
                _propertySetter.SetCreationProperties(e.Entry.Entity);
                break;
            case EntityState.Modified:
                _propertySetter.SetModificationProperties(e.Entry.Entity);
                break;
            case EntityState.Deleted:
                if (e.Entry.Entity is ISoftDelete)
                    e.Entry.Reload();
                _propertySetter.SetDeletionProperties(e.Entry.Entity);
                break;
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Account>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<Transaction>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<Customer>().HasQueryFilter(p => !p.IsDeleted);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
        builder.EnableSensitiveDataLogging();
    }
}