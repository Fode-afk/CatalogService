using CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.DeleteBrandSnapshot;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using Hangfire.Common;
using Hangfire.States;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.BrandSnapshot;

public class DeleteBrandSnapshotCommandHandlerTests
{
    [Fact]
    public async Task Should_Delete_Snapshot_Commit_Transaction_And_Enqueue_Job_When_Snapshot_Found()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var brandSet = new List<CatalogService.Domain.Snapshots.BrandSnapshot>
        {
            SnapshotTestsFactory.AssignableBrand(brandId)
        }.BuildMockDbSet();

        var (context, transaction, backgroundJobClient) = TransactionalHandlerTestsHelper.SetupWithTransaction();
        context.BrandSnapshots.Returns(brandSet);

        var handler = new DeleteBrandSnapshotCommandHandler(context, backgroundJobClient);
        var command = SnapshotCommandTestsFactory.ValidDeleteBrandSnapshotCommand(brandId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        brandSet.Received(1).Remove(Arg.Is<CatalogService.Domain.Snapshots.BrandSnapshot>(s => s.BrandId == brandId));
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        backgroundJobClient.Received(1).Create(
            Arg.Is<Job>(j => j.Type == typeof(ISuspendBrandProductsJob)),
            Arg.Any<IState>());
        await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Do_Nothing_When_Snapshot_Not_Found()
    {
        // Arrange
        var brandSet = new List<CatalogService.Domain.Snapshots.BrandSnapshot>().BuildMockDbSet();

        var (context, _, backgroundJobClient) = TransactionalHandlerTestsHelper.SetupWithTransaction();
        context.BrandSnapshots.Returns(brandSet);

        var handler = new DeleteBrandSnapshotCommandHandler(context, backgroundJobClient);
        var command = SnapshotCommandTestsFactory.ValidDeleteBrandSnapshotCommand();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        brandSet.DidNotReceive().Remove(Arg.Any<CatalogService.Domain.Snapshots.BrandSnapshot>());
        backgroundJobClient.DidNotReceive().Create(Arg.Any<Job>(), Arg.Any<IState>());
        await context.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }
}