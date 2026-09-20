using CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.DeleteVendorSnapshot;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using Hangfire.Common;
using Hangfire.States;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.VendorSnapshot;

public class DeleteVendorSnapshotCommandHandlerTests
{
    [Fact]
    public async Task Should_Delete_Snapshot_Commit_Transaction_And_Enqueue_Job_When_Snapshot_Found()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>
        {
            SnapshotTestsFactory.ActiveVendor(vendorId)
        }.BuildMockDbSet();

        var (context, transaction, backgroundJobClient) = TransactionalHandlerTestsHelper.SetupWithTransaction();
        context.VendorSnapshots.Returns(vendorSet);

        var handler = new DeleteVendorSnapshotCommandHandler(context, backgroundJobClient);
        var command = SnapshotCommandTestsFactory.ValidDeleteVendorSnapshotCommand(vendorId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        vendorSet.Received(1).Remove(Arg.Is<CatalogService.Domain.Snapshots.VendorSnapshot>(s => s.VendorId == vendorId));
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        backgroundJobClient.Received(1).Create(
            Arg.Is<Job>(j => j.Type == typeof(IDeleteVendorProductsJob)),
            Arg.Any<IState>());
        await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Do_Nothing_When_Snapshot_Not_Found()
    {
        // Arrange
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>().BuildMockDbSet();

        var (context, _, backgroundJobClient) = TransactionalHandlerTestsHelper.SetupWithTransaction();
        context.VendorSnapshots.Returns(vendorSet);

        var handler = new DeleteVendorSnapshotCommandHandler(context, backgroundJobClient);
        var command = SnapshotCommandTestsFactory.ValidDeleteVendorSnapshotCommand();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        vendorSet.DidNotReceive().Remove(Arg.Any<CatalogService.Domain.Snapshots.VendorSnapshot>());
        backgroundJobClient.DidNotReceive().Create(Arg.Any<Job>(), Arg.Any<IState>());
        await context.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }
}
