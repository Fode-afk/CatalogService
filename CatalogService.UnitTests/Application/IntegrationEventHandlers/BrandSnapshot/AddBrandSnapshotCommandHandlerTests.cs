using CatalogService.Application.Features.IntegrationEventHandlers.BrandSnapshot.AddBrandSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using MockQueryable.NSubstitute;
using NSubstitute;
using Snapshots = CatalogService.Domain.Snapshots;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.BrandSnapshot;

public class AddBrandSnapshotCommandHandlerTests
{
    [Fact]
    public async Task Should_Add_Snapshot_When_It_Does_Not_Exist()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var brandSet = new List<Snapshots.BrandSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.BrandSnapshots.Returns(brandSet);

        var handler = new AddBrandSnapshotCommandHandler(context, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidAddBrandSnapshotCommand(
            brandId, isAssignable: true, version: 5);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        brandSet.Received(1).Add(Arg.Is<Snapshots.BrandSnapshot>(
            s => s.BrandId == brandId &&
                 s.IsAssignable == true &&
                 s.Version == 5 &&
                 s.UpdatedAt == TestClock.DefaultNow));
    }

    [Fact]
    public async Task Should_Not_Add_Snapshot_When_It_Already_Exists()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var brandSet = new List<Snapshots.BrandSnapshot>
        {
            SnapshotTestsFactory.AssignableBrand(brandId)
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.BrandSnapshots.Returns(brandSet);

        var handler = new AddBrandSnapshotCommandHandler(context, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidAddBrandSnapshotCommand(brandId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        brandSet.DidNotReceive().Add(Arg.Any<Snapshots.BrandSnapshot>());
    }
}
