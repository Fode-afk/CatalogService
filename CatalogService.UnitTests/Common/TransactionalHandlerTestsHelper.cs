using CatalogService.Application.Interfaces.Data;
using Hangfire;
using Microsoft.EntityFrameworkCore.Storage;
using NSubstitute;

namespace CatalogService.UnitTests.Common;

public static class TransactionalHandlerTestsHelper
{
    public static (IAppDbContext Context, IDbContextTransaction Transaction, IBackgroundJobClient JobClient)
        SetupWithTransaction()
    {
        var transaction = Substitute.For<IDbContextTransaction>();

        var context = Substitute.For<IAppDbContext>();
        context.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(transaction);

        var jobClient = Substitute.For<IBackgroundJobClient>();

        return (context, transaction, jobClient);
    }
}