using CatalogService.Application.Interfaces.Data;
using CatalogService.Infrastructure.BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CatalogService.UnitTests.Common;

public static class ServiceScopeFactoryTestsHelper
{
    public static IServiceScopeFactory CreateFor(IAppDbContext context)
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(IAppDbContext)).Returns(context);

        var scope = Substitute.For<IServiceScope>();
        scope.ServiceProvider.Returns(serviceProvider);

        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        scopeFactory.CreateScope().Returns(scope);

        return scopeFactory;
    }

    public static SoftDeletedProductsCleanupOptions ValidCleanupOptions(
        TimeSpan? interval = null,
        TimeSpan? retentionPeriod = null,
        int batchSize = 500) =>
        new()
        {
            Interval = interval ?? TimeSpan.FromHours(6),
            RetentionPeriod = retentionPeriod ?? TimeSpan.FromDays(30),
            BatchSize = batchSize
        };
}
