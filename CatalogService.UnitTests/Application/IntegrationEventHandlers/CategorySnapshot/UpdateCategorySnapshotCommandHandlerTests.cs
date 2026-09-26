using CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.UpdateCategorySnapshot;
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

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.CategorySnapshot;

public class UpdateCategorySnapshotCommandHandlerTests
{
    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();

    [Fact]
    public async Task Should_Update_Snapshot_And_Enqueue_Job_When_IsActive_Changed()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.ActiveCategory(categoryId, version: 1);
        existingSnapshot.IsActive = false;

        var categorySet = new List<CatalogService.Domain.Snapshots.CategorySnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var (context, transaction, backgroundJobClient) = TransactionalHandlerTestsHelper.SetupWithTransaction();
        context.CategorySnapshots.Returns(categorySet);

        var handler = new UpdateCategorySnapshotCommandHandler(
            context, backgroundJobClient, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateCategorySnapshotCommand(
            categoryId, isActive: true, version: 2);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        existingSnapshot.IsActive.Should().BeTrue();
        existingSnapshot.Version.Should().Be(2);
        existingSnapshot.UpdatedAt.Should().Be(TestClock.DefaultNow);

        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        backgroundJobClient.Received(1).Create(
            Arg.Is<Job>(j => j.Type == typeof(ISuspendCategoryProductsJob)),
            Arg.Any<IState>());
        await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Update_Snapshot_Without_Enqueueing_Job_When_IsActive_Not_Changed()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.ActiveCategory(categoryId, version: 1);
        existingSnapshot.IsActive = true;

        var categorySet = new List<CatalogService.Domain.Snapshots.CategorySnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var (context, transaction, backgroundJobClient) = TransactionalHandlerTestsHelper.SetupWithTransaction();
        context.CategorySnapshots.Returns(categorySet);

        var handler = new UpdateCategorySnapshotCommandHandler(
            context, backgroundJobClient, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateCategorySnapshotCommand(
            categoryId, isActive: true, version: 2);

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
        var categorySet = new List<CatalogService.Domain.Snapshots.CategorySnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.CategorySnapshots.Returns(categorySet);
        var backgroundJobClient = Substitute.For<IBackgroundJobClient>();

        var handler = new UpdateCategorySnapshotCommandHandler(
            context, backgroundJobClient, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateCategorySnapshotCommand();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SnapshotNotFoundException>();
        _metrics.Received(1).RecordSnapshotNotFound("Category");
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        backgroundJobClient.DidNotReceive().Create(Arg.Any<Job>(), Arg.Any<IState>());
    }

    [Fact]
    public async Task Should_Skip_Update_And_Record_Metric_When_Version_Is_Outdated()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.ActiveCategory(categoryId, version: 5);

        var categorySet = new List<CatalogService.Domain.Snapshots.CategorySnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.CategorySnapshots.Returns(categorySet);
        var backgroundJobClient = Substitute.For<IBackgroundJobClient>();

        var handler = new UpdateCategorySnapshotCommandHandler(
            context, backgroundJobClient, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateCategorySnapshotCommand(
            categoryId, version: 5);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordSnapshotOutdated(nameof(UpdateCategorySnapshotCommand));
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        backgroundJobClient.DidNotReceive().Create(Arg.Any<Job>(), Arg.Any<IState>());
        await context.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Skip_Update_When_Version_Is_Strictly_Older()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.ActiveCategory(categoryId, version: 5);

        var categorySet = new List<CatalogService.Domain.Snapshots.CategorySnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.CategorySnapshots.Returns(categorySet);
        var backgroundJobClient = Substitute.For<IBackgroundJobClient>();

        var handler = new UpdateCategorySnapshotCommandHandler(
            context, backgroundJobClient, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateCategorySnapshotCommand(
            categoryId, version: 3);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordSnapshotOutdated(nameof(UpdateCategorySnapshotCommand));
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}