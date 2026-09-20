using CatalogService.Application.Features.IntegrationEventHandlers.ProductVariantPriceSnapshot.AddProductVariantPriceSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.ProductVariantPriceSnapshot;

public class AddProductVariantPriceSnapshotCommandHandlerTests
{
    [Fact]
    public async Task Should_Add_Snapshot_When_It_Does_Not_Exist()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);

        var handler = new AddProductVariantPriceSnapshotCommandHandler(context, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidAddProductVariantPriceSnapshotCommand(
            productVariantId, hasPrice: true, version: 5);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        priceSet.Received(1).Add(Arg.Is<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot>(
            s => s.ProductVariantId == productVariantId &&
                 s.HasPrice == true &&
                 s.Version == 5 &&
                 s.UpdatedAt == TestClock.DefaultNow));
    }

    [Fact]
    public async Task Should_Not_Add_Snapshot_When_It_Already_Exists()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();
        var priceSet = new List<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot>
        {
            SnapshotTestsFactory.PriceSnapshot(productVariantId: productVariantId)
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.ProductVariantPriceSnapshots.Returns(priceSet);

        var handler = new AddProductVariantPriceSnapshotCommandHandler(context, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidAddProductVariantPriceSnapshotCommand(productVariantId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        priceSet.DidNotReceive().Add(Arg.Any<CatalogService.Domain.Snapshots.ProductVariantPriceSnapshot>());
    }
}