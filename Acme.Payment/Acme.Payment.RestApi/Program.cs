using Acme.Foundation.Application.Services;
using Acme.Foundation.Domain.Auditing;
using Acme.Foundation.Domain.Entities;
using Acme.Payment.Domain.Repositories;
using Acme.Payment.EfCore.DbContext;
using Acme.Payment.EfCore.Repositories;
using Acme.Payment.RestApi.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Acme.Payment.RestApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var app = BuildWebApplication(args);
            await ConfigureWebApplication(app)
                .RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Web application terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static WebApplication BuildWebApplication(string[] args)
    {
        //Configure Serilog
        StartupHelper.ConfigureLogger("Acme.Payment");

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        // Add Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add Payment In Memory Database 
        builder.Services.AddDbContext<PaymentDbContext>(
            opt => opt.UseInMemoryDatabase("PaymentDatabase"));

        // Register Mapper Profile
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Add Repositories
        builder.Services.AddTransient<ICustomerRepository>(s =>
        {
            var dbContext = s.GetRequiredService<PaymentDbContext>();
            return new CustomerRepository(dbContext);
        });
        builder.Services.AddTransient<IAccountRepository>(s =>
        {
            var dbContext = s.GetRequiredService<PaymentDbContext>();
            return new AccountRepository(dbContext);
        });
        builder.Services.AddTransient<ITransactionRepository>(s =>
        {
            var dbContext = s.GetRequiredService<PaymentDbContext>();
            return new TransactionRepository(dbContext);
        });
        builder.Services.AddTransient<IAdjustmentRepository>(s =>
        {
            var dbContext = s.GetRequiredService<PaymentDbContext>();
            return new AdjustmentRepository(dbContext);
        });

        // Add Application Services 
        builder.Services.AddTransient<ICustomerAppService, CustomerAppService>();
        builder.Services.AddTransient<IAccountAppService, AccountAppService>();
        builder.Services.AddTransient<ITransactionAppService, TransactionAppService>();

        // Add Auto Auditing 
        builder.Services.AddTransient<IAuditPropertySetter, AuditPropertySetter>();
        builder.Services.AddSingleton<ICurrentUser>(
            _ => new CurrentUser { Id = Guid.NewGuid(), IsAuthenticated = true, Name = "Admin" });

        return builder.Build();
    }

    private static WebApplication ConfigureWebApplication(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        // global error handler
        app.UseMiddleware<ErrorHandlerMiddleware>();

        app.MapControllers();

        // Seed initial data
        StartupHelper.SeedDatabase(app.Services);

        return app;
    }
}
