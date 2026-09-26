using CatalogService.Domain.Snapshots;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.IntegrationTests.Infrastructure.Data;

public class ProductVariantSnapshotTests : IAsyncLifetime
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();

    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_RoundTrip_ProductVariantSnapshot()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.VariantSnapshot();

        _fixture.Context.ProductVariantSnapshots.Add(snapshot);

        // Act
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        _fixture.Context.ChangeTracker.Clear();

        var reloaded = await _fixture.Context.ProductVariantSnapshots
            .FirstAsync(
                x => x.ProductVariantId == snapshot.ProductVariantId,
                cancellationToken);

        // Assert
        reloaded.ProductVariantId.Should().Be(snapshot.ProductVariantId);
        reloaded.ProductId.Should().Be(snapshot.ProductId);
        reloaded.HasMainImage.Should().Be(snapshot.HasMainImage);
        reloaded.UpdatedAt.Should().Be(snapshot.UpdatedAt);
        reloaded.Version.Should().Be(snapshot.Version);
    }

    [Fact]
    public async Task Should_Reject_Duplicate_ProductVariantId()
    {
        // Arrange
        var productVariantId = Guid.NewGuid();

        var snapshot1 = SnapshotTestsFactory.VariantSnapshot(productVariantId: productVariantId);
        var snapshot2 = SnapshotTestsFactory.VariantSnapshot(productVariantId: productVariantId, hasMainImage: false);

        _fixture.Context.ProductVariantSnapshots.Add(snapshot1);

        await _fixture.Context.SaveChangesAsync(cancellationToken);

        _fixture.Context.ChangeTracker.Clear();

        _fixture.Context.ProductVariantSnapshots.Add(snapshot2);

        // Act
        var act = () =>
            _fixture.Context.SaveChangesAsync(cancellationToken);

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public void Should_Have_Index_On_ProductId()
    {
        // Act
        var entityType = _fixture.Context.Model
            .FindEntityType(typeof(ProductVariantSnapshot));

        var index = entityType!.FindIndex(
            entityType.FindProperty(
                nameof(ProductVariantSnapshot.ProductId))!);

        // Assert
        index.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Generate_RowVersion_When_Snapshot_Is_Saved()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.VariantSnapshot();

        _fixture.Context.ProductVariantSnapshots.Add(snapshot);

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