using CatalogService.Application.Features.IntegrationEventHandlers.CategorySnapshot.AddCategorySnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.CategorySnapshot;

public class AddCategorySnapshotCommandHandlerTests
{
    [Fact]
    public async Task Should_Add_Snapshot_When_It_Does_Not_Exist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categorySet = new List<CatalogService.Domain.Snapshots.CategorySnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.CategorySnapshots.Returns(categorySet);

        var handler = new AddCategorySnapshotCommandHandler(context, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidAddCategorySnapshotCommand(
            categoryId, isActive: true, version: 5);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        categorySet.Received(1).Add(Arg.Is<CatalogService.Domain.Snapshots.CategorySnapshot>(
            s => s.CategoryId == categoryId &&
                 s.IsActive == true &&
                 s.Version == 5 &&
                 s.UpdatedAt == TestClock.DefaultNow));
    }

    [Fact]
    public async Task Should_Not_Add_Snapshot_When_It_Already_Exists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var categorySet = new List<CatalogService.Domain.Snapshots.CategorySnapshot>
        {
            SnapshotTestsFactory.ActiveCategory(categoryId)
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.CategorySnapshots.Returns(categorySet);

        var handler = new AddCategorySnapshotCommandHandler(context, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidAddCategorySnapshotCommand(categoryId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        categorySet.DidNotReceive().Add(Arg.Any<CatalogService.Domain.Snapshots.CategorySnapshot>());
    }
}
