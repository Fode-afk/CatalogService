using CatalogService.Domain.Snapshots;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.IntegrationTests.Infrastructure.Data;

public class CategorySnapshotTests : IAsyncLifetime
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_RoundTrip_CategorySnapshot()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.ActiveCategory();

        _fixture.Context.CategorySnapshots.Add(snapshot);

        // Act
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        _fixture.Context.ChangeTracker.Clear();

        var reloaded = await _fixture.Context.CategorySnapshots
            .FirstAsync(
                x => x.CategoryId == snapshot.CategoryId,
                cancellationToken);

        // Assert
        reloaded.CategoryId.Should().Be(snapshot.CategoryId);
        reloaded.IsActive.Should().Be(snapshot.IsActive);
        reloaded.UpdatedAt.Should().Be(snapshot.UpdatedAt);
        reloaded.Version.Should().Be(snapshot.Version);
    }

    [Fact]
    public async Task Should_Generate_RowVersion_When_Snapshot_Is_Saved()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.ActiveCategory();

        _fixture.Context.CategorySnapshots.Add(snapshot);

        // Act
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        // Assert
        var rowVersion = _fixture.Context.Entry(snapshot)
            .Property<byte[]>("RowVersion")
            .CurrentValue;

        rowVersion.Should().NotBeNull();
        rowVersion.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_Reject_Duplicate_CategoryId()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var snapshot1 = SnapshotTestsFactory.ActiveCategory(categoryId);
        var snapshot2 = SnapshotTestsFactory.InactiveCategory(categoryId);

        _fixture.Context.CategorySnapshots.Add(snapshot1);

        await _fixture.Context.SaveChangesAsync(cancellationToken);

        _fixture.Context.ChangeTracker.Clear();

        _fixture.Context.CategorySnapshots.Add(snapshot2);

        // Act
        var act = () => _fixture.Context.SaveChangesAsync(cancellationToken);

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public void Should_Have_Index_On_IsActive()
    {
        // Act
        var entityType = _fixture.Context.Model
            .FindEntityType(typeof(CategorySnapshot));

        var index = entityType!.FindIndex(
            entityType.FindProperty(nameof(CategorySnapshot.IsActive))!);

        // Assert
        index.Should().NotBeNull();
    }
}
