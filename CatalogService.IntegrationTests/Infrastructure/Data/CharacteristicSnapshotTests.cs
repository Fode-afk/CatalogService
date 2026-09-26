using CatalogService.Domain.Snapshots;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Enums.Characteristics;

namespace CatalogService.IntegrationTests.Infrastructure.Data;

public class CharacteristicSnapshotTests : IAsyncLifetime
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_RoundTrip_CharacteristicSnapshot()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.Characteristic(groupName: "Appearance", isUnifying: true);

        _fixture.Context.CharacteristicSnapshots.Add(snapshot);

        // Act
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        _fixture.Context.ChangeTracker.Clear();

        var reloaded = await _fixture.Context.CharacteristicSnapshots
            .FirstAsync(
                x => x.CharacteristicId == snapshot.CharacteristicId,
                cancellationToken);

        // Assert
        reloaded.CharacteristicId.Should().Be(snapshot.CharacteristicId);
        reloaded.Name.Should().Be(snapshot.Name);
        reloaded.CharType.Should().Be(snapshot.CharType);
        reloaded.GroupName.Should().Be(snapshot.GroupName);
        reloaded.IsUnifying.Should().Be(snapshot.IsUnifying);
        reloaded.CategoryId.Should().Be(snapshot.CategoryId);
        reloaded.UpdatedAt.Should().Be(snapshot.UpdatedAt);
        reloaded.Version.Should().Be(snapshot.Version);
    }

    [Fact]
    public async Task Should_Store_CharType_As_String()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.Characteristic();

        _fixture.Context.CharacteristicSnapshots.Add(snapshot);

        // Act
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        // Assert
        var storedValue = await _fixture.Context.Database
            .SqlQuery<string>(
                $"""
                SELECT [CharType]
                FROM [catalog_write].[CharacteristicSnapshots]
                WHERE [CharacteristicId] = {snapshot.CharacteristicId}
                """)
            .SingleAsync(cancellationToken);

        storedValue.Should().Be(snapshot.CharType.ToString());
    }

    [Fact]
    public async Task Should_Reject_Characteristic_Without_Name()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.Characteristic(name: null!);

        _fixture.Context.CharacteristicSnapshots.Add(snapshot);

        // Act
        var act = () =>
            _fixture.Context.SaveChangesAsync(cancellationToken);

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Should_Reject_Name_Longer_Than_256_Characters()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.Characteristic(name: new string('a', 257));

        _fixture.Context.CharacteristicSnapshots.Add(snapshot);

        // Act
        var act = () =>
            _fixture.Context.SaveChangesAsync(cancellationToken);

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public void Should_Have_Index_On_CategoryId()
    {
        // Act
        var entityType = _fixture.Context.Model
            .FindEntityType(typeof(CharacteristicSnapshot));

        var index = entityType!.FindIndex(
            entityType.FindProperty(
                nameof(CharacteristicSnapshot.CategoryId))!);

        // Assert
        index.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Generate_RowVersion_When_Snapshot_Is_Saved()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.Characteristic();

        _fixture.Context.CharacteristicSnapshots.Add(snapshot);

        // Act
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        // Assert
        var rowVersion = _fixture.Context.Entry(snapshot)
            .Property<byte[]>("RowVersion")
            .CurrentValue;

        rowVersion.Should().NotBeNull();
        rowVersion.Should().NotBeEmpty();
    }
}