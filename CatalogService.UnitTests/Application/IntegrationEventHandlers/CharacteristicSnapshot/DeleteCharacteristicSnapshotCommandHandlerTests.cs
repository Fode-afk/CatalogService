using CatalogService.Application.Features.IntegrationEventHandlers.CharacteristicSnapshot.DeleteCharacteristicSnapshot;
using CatalogService.Application.Interfaces.Data;
using CatalogService.UnitTests.Fixtures;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CatalogService.UnitTests.Application.IntegrationEventHandlers.CharacteristicSnapshot;

public class DeleteCharacteristicSnapshotCommandHandlerTests
{
    [Fact]
    public async Task Should_Delete_Snapshot_When_It_Exists()
    {
        // Arrange
        var characteristicId = Guid.NewGuid();
        var characteristicSet = new List<CatalogService.Domain.Snapshots.CharacteristicSnapshot>
        {
            SnapshotTestsFactory.Characteristic(characteristicId)
        }.BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.CharacteristicSnapshots.Returns(characteristicSet);

        var handler = new DeleteCharacteristicSnapshotCommandHandler(context);
        var command = SnapshotCommandTestsFactory.ValidDeleteCharacteristicSnapshotCommand(characteristicId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        characteristicSet.Received(1).Remove(Arg.Is<CatalogService.Domain.Snapshots.CharacteristicSnapshot>(
            s => s.CharacteristicId == characteristicId));
        await context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Do_Nothing_When_Snapshot_Not_Found()
    {
        // Arrange
        var characteristicSet = new List<CatalogService.Domain.Snapshots.CharacteristicSnapshot>().BuildMockDbSet();

        var context = Substitute.For<IAppDbContext>();
        context.CharacteristicSnapshots.Returns(characteristicSet);

        var handler = new DeleteCharacteristicSnapshotCommandHandler(context);
        var command = SnapshotCommandTestsFactory.ValidDeleteCharacteristicSnapshotCommand();

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        characteristicSet.DidNotReceive().Remove(Arg.Any<CatalogService.Domain.Snapshots.CharacteristicSnapshot>());
    }
}
