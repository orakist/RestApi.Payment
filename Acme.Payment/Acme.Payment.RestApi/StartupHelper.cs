using Acme.Foundation.Domain.Entities;
using Acme.Payment.Domain.Entities;
using Acme.Payment.EfCore.DbContext;
using Serilog;
using Serilog.Events;

namespace Acme.Payment.RestApi;

public static class StartupHelper
{
    public static void ConfigureLogger(string applicationName)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName)
#if DEBUG
            .WriteTo.Async(
                c => c.File(
                    path: "Logs.Web/logs.txt",
                    fileSizeLimitBytes: 4L * 1024 * 1024 * 1024,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 365,
                    rollOnFileSizeLimit: true)
            )
#endif
            .WriteTo.Async(c => c.Console())
            .CreateLogger();
    }

    public static void SeedDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var user = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

        var cust0 = new Customer(user.Id ?? Guid.NewGuid(), user.Name);
        var cust1 = new Customer(Guid.NewGuid(), "Orhan");
        var cust2 = new Customer(Guid.NewGuid(), "Mehmet");
        var cust3 = new Customer(Guid.NewGuid(), "Metin");

        var acc1 = new Account(Guid.NewGuid(), 4755, "Current Account", 1001.88m, cust1.Id);
        var acc2 = new Account(Guid.NewGuid(), 9834, "Current Account", 456.45m, cust2.Id);
        var acc3 = new Account(Guid.NewGuid(), 7735, "Current Account", 89.36m, cust3.Id);

        dbContext.Customers.AddRange(cust0, cust1, cust2, cust3);
        dbContext.Accounts.AddRange(acc1, acc2, acc3);

        dbContext.SaveChanges();
    }
}
