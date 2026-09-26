using CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.UpdateVendorSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Jobs;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Exceptions;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using CatalogService.UnitTests.Common;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.VendorSnapshot;

public class UpdateVendorSnapshotCommandHandlerTests
{
    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();

    [Fact]
    public async Task Should_Update_Snapshot_And_Enqueue_Job_When_IsActive_Changed()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.ActiveVendor(vendorId, version: 1);
        existingSnapshot.IsActive = false;

        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var (context, transaction, backgroundJobClient) = TransactionalHandlerTestsHelper.SetupWithTransaction();
        context.VendorSnapshots.Returns(vendorSet);

        var handler = new UpdateVendorSnapshotCommandHandler(
            context, backgroundJobClient, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateVendorSnapshotCommand(
            vendorId, isActive: true, version: 2);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        existingSnapshot.IsActive.Should().BeTrue();
        existingSnapshot.Version.Should().Be(2);
        existingSnapshot.UpdatedAt.Should().Be(TestClock.DefaultNow);

        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        backgroundJobClient.Received(1).Create(
            Arg.Is<Job>(j => j.Type == typeof(ISuspendVendorProductsJob)),
            Arg.Any<IState>());
        await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Update_Snapshot_Without_Enqueueing_Job_When_IsActive_Not_Changed()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.ActiveVendor(vendorId, version: 1);
        existingSnapshot.IsActive = true;

        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var (context, transaction, backgroundJobClient) = TransactionalHandlerTestsHelper.SetupWithTransaction();
        context.VendorSnapshots.Returns(vendorSet);

        var handler = new UpdateVendorSnapshotCommandHandler(
            context, backgroundJobClient, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateVendorSnapshotCommand(
            vendorId, isActive: true, version: 2);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        backgroundJobClient.DidNotReceive().Create(Arg.Any<Job>(), Arg.Any<IState>());
        await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Throw_And_Record_Metric_When_Snapshot_Not_Found()
    {
        // Arrange
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.VendorSnapshots.Returns(vendorSet);
        var backgroundJobClient = Substitute.For<IBackgroundJobClient>();

        var handler = new UpdateVendorSnapshotCommandHandler(
            context, backgroundJobClient, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateVendorSnapshotCommand();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SnapshotNotFoundException>();
        _metrics.Received(1).RecordSnapshotNotFound("Vendor");
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        backgroundJobClient.DidNotReceive().Create(Arg.Any<Job>(), Arg.Any<IState>());
    }

    [Fact]
    public async Task Should_Skip_Update_And_Record_Metric_When_Version_Is_Outdated()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.ActiveVendor(vendorId, version: 5);

        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.VendorSnapshots.Returns(vendorSet);
        var backgroundJobClient = Substitute.For<IBackgroundJobClient>();

        var handler = new UpdateVendorSnapshotCommandHandler(
            context, backgroundJobClient, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateVendorSnapshotCommand(
            vendorId, version: 5);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordSnapshotOutdated(nameof(UpdateVendorSnapshotCommand));
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        backgroundJobClient.DidNotReceive().Create(Arg.Any<Job>(), Arg.Any<IState>());
        await context.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Skip_Update_When_Version_Is_Strictly_Older()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.ActiveVendor(vendorId, version: 5);

        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.VendorSnapshots.Returns(vendorSet);
        var backgroundJobClient = Substitute.For<IBackgroundJobClient>();

        var handler = new UpdateVendorSnapshotCommandHandler(
            context, backgroundJobClient, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateVendorSnapshotCommand(
            vendorId, version: 3);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordSnapshotOutdated(nameof(UpdateVendorSnapshotCommand));
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}