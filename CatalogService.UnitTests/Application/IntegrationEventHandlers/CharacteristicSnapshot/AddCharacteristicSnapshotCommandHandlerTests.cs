using CatalogService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.AddCharacteristicSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.TestCommon;
using CatalogService.TestCommon.Fixtures;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.CharacteristicSnapshot;

public class AddCharacteristicSnapshotCommandHandlerTests
{
    [Fact]
    public async Task Should_Add_Snapshot_When_It_Does_Not_Exist()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var characteristicSet = new List<CatalogService.Domain.Snapshots.CharacteristicSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.CharacteristicSnapshots.Returns(characteristicSet);

        var handler = new AddCharacteristicSnapshotCommandHandler(context, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidAddCharacteristicSnapshotCommand(
            characteristicId, name: "Color", groupName: "Basic", isUnifying: true,
            categoryId: categoryId, version: 5);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        characteristicSet.Received(1).Add(Arg.Is<CatalogService.Domain.Snapshots.CharacteristicSnapshot>(
            s => s.CharacteristicId == characteristicId &&
                 s.Name == "Color" &&
                 s.GroupName == "Basic" &&
                 s.IsUnifying == true &&
                 s.CategoryId == categoryId &&
                 s.Version == 5 &&
                 s.UpdatedAt == TestClock.DefaultNow));
    }

    [Fact]
    public async Task Should_Not_Add_Snapshot_When_It_Already_Exists()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var characteristicSet = new List<CatalogService.Domain.Snapshots.CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(characteristicId)
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.CharacteristicSnapshots.Returns(characteristicSet);

        var handler = new AddCharacteristicSnapshotCommandHandler(context, TestClock.Create());
        var command = SnapshotCommandTestsFactory.ValidAddCharacteristicSnapshotCommand(characteristicId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        characteristicSet.DidNotReceive().Add(Arg.Any<CatalogService.Domain.Snapshots.CharacteristicSnapshot>());
    }
}