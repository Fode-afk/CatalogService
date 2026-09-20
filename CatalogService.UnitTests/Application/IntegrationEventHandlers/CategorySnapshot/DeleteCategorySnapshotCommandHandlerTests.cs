using CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.DeleteCategorySnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.CategorySnapshot;

public class DeleteCategorySnapshotCommandHandlerTests
{
    [Fact]
    public async Task Should_Delete_Snapshot_Commit_Transaction_And_Enqueue_Job_When_Snapshot_Found()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categorySet = new List<CatalogService.Domain.Snapshots.CategorySnapshot>
        {
            SnapshotTestsFactory.ActiveCategory(categoryId)
        }.BuildMockDbSet();

        var (context, transaction, backgroundJobClient) = TransactionalHandlerTestsHelper.SetupWithTransaction();
        context.CategorySnapshots.Returns(categorySet);

        var handler = new DeleteCategorySnapshotCommandHandler(context, backgroundJobClient);
        var command = SnapshotCommandTestsFactory.ValidDeleteCategorySnapshotCommand(categoryId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        categorySet.Received(1).Remove(Arg.Is<CatalogService.Domain.Snapshots.CategorySnapshot>(s => s.CategoryId == categoryId));
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        backgroundJobClient.Received(1).Create(
            Arg.Is<Job>(j => j.Type == typeof(ISuspendCategoryProductsJob)),
            Arg.Any<IState>());
        await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Do_Nothing_When_Snapshot_Not_Found()
    {
        // Arrange
        var categorySet = new List<CatalogService.Domain.Snapshots.CategorySnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.CategorySnapshots.Returns(categorySet);

        var backgroundJobClient = Substitute.For<IBackgroundJobClient>();

        var handler = new DeleteCategorySnapshotCommandHandler(context, backgroundJobClient);
        var command = SnapshotCommandTestsFactory.ValidDeleteCategorySnapshotCommand();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        categorySet.DidNotReceive().Remove(Arg.Any<CatalogService.Domain.Snapshots.CategorySnapshot>());
        backgroundJobClient.DidNotReceive().Create(Arg.Any<Job>(), Arg.Any<IState>());
        await context.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }
}