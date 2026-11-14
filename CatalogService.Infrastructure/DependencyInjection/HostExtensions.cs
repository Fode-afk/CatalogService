using CatalogService.Application.Interfaces.Jobs;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Jobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CatalogService.Infrastructure.DependencyInjection;

public static class HostExtensions
{
    public static async Task MigrateDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    public static async Task SeedDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DbContextSeed.SeedAsync(db);
    }

    public static IHost RegisterRecurringJobs(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var sp = scope.ServiceProvider;

        var recurringJobs = sp.GetRequiredService<IRecurringJobManager>();
        var cleanupOptions = sp.GetRequiredService<SoftDeletedProductsCleanupOptions>();

        recurringJobs.AddOrUpdate<ICleanupSoftDeletedProductsJob>(
            recurringJobId: "cleanup-soft-deleted-products",
            methodCall: job => job.Execute(CancellationToken.None),
            cronExpression: cleanupOptions.CronExpression);

        return host;
    }
}