using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantSnapshot.UpdateProductVariantSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.Application.Interfaces.Metrics;
using CatalogService.Domain.Exceptions;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.ProductVariantSnapshot;

public class UpdateProductVariantSnapshotCommandHandlerTests
{
    private readonly ICatalogMetrics _metrics = Substitute.For<ICatalogMetrics>();

    [Fact]
    public async Task Should_Update_Snapshot_When_Version_Is_Newer()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.VariantSnapshot(
            productVariantId: productVariantId, hasMainImage: false, version: 1);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);

        var handler = new UpdateProductVariantSnapshotCommandHandler(context, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateProductVariantSnapshotCommand(
            productVariantId, hasMainImage: true, version: 2);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        existingSnapshot.HasMainImage.Should().BeTrue();
        existingSnapshot.Version.Should().Be(2);
        existingSnapshot.UpdatedAt.Should().Be(TestClock.DefaultNow);
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Throw_And_Record_Metric_When_Snapshot_Not_Found()
    {
        // Arrange
        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);

        var handler = new UpdateProductVariantSnapshotCommandHandler(context, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateProductVariantSnapshotCommand();

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SnapshotNotFoundException>();
        _metrics.Received(1).RecordSnapshotNotFound("ProductVariant");
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Skip_Update_And_Record_Metric_When_Version_Is_Outdated()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.VariantSnapshot(
            productVariantId: productVariantId, version: 5);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);

        var handler = new UpdateProductVariantSnapshotCommandHandler(context, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateProductVariantSnapshotCommand(
            productVariantId, version: 5);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordSnapshotOutdated(nameof(UpdateProductVariantSnapshotCommand));
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Skip_Update_When_Version_Is_Strictly_Older()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var existingSnapshot = SnapshotTestsFactory.VariantSnapshot(
            productVariantId: productVariantId, version: 5);

        var variantSet = new List<CatalogService.Domain.Snapshots.ProductVariantSnapshot> { existingSnapshot }
            .BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantSnapshots.Returns(variantSet);

        var handler = new UpdateProductVariantSnapshotCommandHandler(context, _metrics, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidUpdateProductVariantSnapshotCommand(
            productVariantId, version: 3);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _metrics.Received(1).RecordSnapshotOutdated(nameof(UpdateProductVariantSnapshotCommand));
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}