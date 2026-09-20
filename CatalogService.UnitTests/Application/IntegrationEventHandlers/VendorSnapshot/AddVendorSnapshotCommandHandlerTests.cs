using CatalogService.Application.Features.IntegrationEventHandlers.VendorSnapshot.AddVendorSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.UnitTests.Common;
using CatalogService.UnitTests.Fixtures;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.VendorSnapshot;

public class AddVendorSnapshotCommandHandlerTests
{
    [Fact]
    public async Task Should_Add_Snapshot_When_It_Does_Not_Exist()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.VendorSnapshots.Returns(vendorSet);

        var handler = new AddVendorSnapshotCommandHandler(context, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidAddVendorSnapshotCommand(
            vendorId, isActive: true, version: 5);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        vendorSet.Received(1).Add(Arg.Is<CatalogService.Domain.Snapshots.VendorSnapshot>(
            s => s.VendorId == vendorId &&
                 s.IsActive == true &&
                 s.Version == 5 &&
                 s.UpdatedAt == TestClock.DefaultNow));
    }

    [Fact]
    public async Task Should_Not_Add_Snapshot_When_It_Already_Exists()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var vendorSet = new List<CatalogService.Domain.Snapshots.VendorSnapshot>
        {
            SnapshotTestsFactory.ActiveVendor(vendorId)
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.VendorSnapshots.Returns(vendorSet);

        var handler = new AddVendorSnapshotCommandHandler(context, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidAddVendorSnapshotCommand(vendorId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        vendorSet.DidNotReceive().Add(Arg.Any<CatalogService.Domain.Snapshots.VendorSnapshot>());
    }
}