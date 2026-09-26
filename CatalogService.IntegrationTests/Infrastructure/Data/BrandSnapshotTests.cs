using CatalogService.Domain.Snapshots;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.IntegrationTests.Infrastructure.Data;

public class BrandSnapshotTests : IAsyncLifetime
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();
    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_RoundTrip_BrandSnapshot()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.AssignableBrand();

        _fixture.Context.BrandSnapshots.Add(snapshot);

        // Act
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        _fixture.Context.ChangeTracker.Clear();

        var reloaded = await _fixture.Context.BrandSnapshots
            .FirstAsync(
                x => x.BrandId == snapshot.BrandId,
                cancellationToken);

        // Assert
        reloaded.BrandId.Should().Be(snapshot.BrandId);
        reloaded.IsAssignable.Should().Be(snapshot.IsAssignable);
        reloaded.UpdatedAt.Should().Be(snapshot.UpdatedAt);
        reloaded.Version.Should().Be(snapshot.Version);
    }

    [Fact]
    public async Task Should_Generate_RowVersion_When_Snapshot_Is_Saved()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.AssignableBrand();

        _fixture.Context.BrandSnapshots.Add(snapshot);

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
    public async Task Should_Reject_Duplicate_BrandId()
    {
        // Arrange
        var brandId = Guid.NewGuid();

        var snapshot1 = SnapshotTestsFactory.AssignableBrand(brandId);
        var snapshot2 = SnapshotTestsFactory.AssignableBrand(brandId);

        _fixture.Context.BrandSnapshots.Add(snapshot1);
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        _fixture.Context.ChangeTracker.Clear();

        _fixture.Context.BrandSnapshots.Add(snapshot2);

        // Act
        var act = () =>
            _fixture.Context.SaveChangesAsync(cancellationToken);

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public void Should_Have_Index_On_IsAssignable()
    {
        // Act
        var entityType = _fixture.Context.Model
            .FindEntityType(typeof(BrandSnapshot));

        var index = entityType!.FindIndex(
            entityType.FindProperty(nameof(BrandSnapshot.IsAssignable))!);

        // Assert
        index.Should().NotBeNull();
    }
}
