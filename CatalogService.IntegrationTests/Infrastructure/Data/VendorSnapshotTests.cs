using CatalogService.Infrastructure.Data;
using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.IntegrationTests.Infrastructure.Data;

public class VendorSnapshotTests : IAsyncLifetime
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();
    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_RoundTrip_VendorSnapshot()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.ActiveVendor();

        _fixture.Context.VendorSnapshots.Add(snapshot);

        // Act
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        _fixture.Context.ChangeTracker.Clear();

        var reloaded = await _fixture.Context.VendorSnapshots
            .FirstAsync(
                v => v.VendorId == snapshot.VendorId,
                cancellationToken);

        // Assert
        reloaded.VendorId.Should().Be(snapshot.VendorId);
        reloaded.UpdatedAt.Should().Be(snapshot.UpdatedAt);
        reloaded.IsActive.Should().Be(snapshot.IsActive);
        reloaded.Version.Should().Be(snapshot.Version);
    }

    [Fact]
    public async Task Should_Generate_RowVersion_When_Snapshot_Is_Saved()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.ActiveVendor();

        _fixture.Context.VendorSnapshots.Add(snapshot);

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
    public async Task Should_Reject_Duplicate_VendorId()
    {
        // Arrange
        var vendorId = Guid.NewGuid();
        var snapshot1 = SnapshotTestsFactory.ActiveVendor(vendorId);
        var snapshot2 = SnapshotTestsFactory.InactiveVendor(vendorId);

        _fixture.Context.VendorSnapshots.Add(snapshot1);
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        _fixture.Context.ChangeTracker.Clear();

        _fixture.Context.VendorSnapshots.Add(snapshot2);

        // Act
        var act = () => _fixture.Context.SaveChangesAsync(cancellationToken);

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task Should_Throw_When_Concurrent_Update_Occurs()
    {
        // Arrange
        var snapshot = SnapshotTestsFactory.ActiveVendor();

        _fixture.Context.VendorSnapshots.Add(snapshot);
        await _fixture.Context.SaveChangesAsync(cancellationToken);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .Options;

        await using var secondContext =
            new AppDbContext(options, _fixture.Dispatcher);

        var firstSnapshot = await _fixture.Context.VendorSnapshots
            .FirstAsync(
                v => v.VendorId == snapshot.VendorId,
                cancellationToken);

        var secondSnapshot = await secondContext.VendorSnapshots
            .FirstAsync(
                v => v.VendorId == snapshot.VendorId,
                cancellationToken);

        // Act
        firstSnapshot.IsActive = false;

        await _fixture.Context.SaveChangesAsync(cancellationToken);

        secondSnapshot.IsActive = false;

        var act = () => secondContext.SaveChangesAsync(cancellationToken);

        // Assert
        await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }
}