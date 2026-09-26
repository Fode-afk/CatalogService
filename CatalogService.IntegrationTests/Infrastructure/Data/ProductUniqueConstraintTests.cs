using CatalogService.IntegrationTests.Common;
using CatalogService.TestCommon.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.IntegrationTests.Infrastructure.Data;

public class ProductUniqueConstraintTests : IAsyncLifetime
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private readonly MsSqlAppDbContextFixture _fixture = new();

    public ValueTask InitializeAsync() => _fixture.InitializeAsync();
    public ValueTask DisposeAsync() => _fixture.DisposeAsync();

    [Fact]
    public async Task Should_Throw_When_Two_Products_Have_Same_Slug()
    {
        var product1 = ProductTestFactory.CreateValid();
        var product2 = ProductTestFactory.CreateValid();

        _fixture.Context.Products.Add(product1);
        await _fixture.Context.SaveChangesAsync(cancellationToken);
        _fixture.Context.ChangeTracker.Clear();

        _fixture.Context.Products.Add(product2);

        // Act
        var act = () => _fixture.Context.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<DbUpdateException>();
    }
}