using CatalogService.Application.Interfaces.Data;
using CatalogService.Domain.Models;
using Dapper;

namespace CatalogService.Infrastructure.Data.Repositories;

internal sealed class ProductReadRepository(IDbConnectionFactory connectionFactory) : IProductReadRepository
{
    public async Task<ProductReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                Id,
                Name,
                Slug,
                Description,
                ShortDescription,

                RatingAvg,
                RatingCount,

                DefaultProductId,
                ProductCount,

                PriceAmount,
                OldPriceAmount,
                PriceUpdatedAt,

                StockStatus,
                StockUpdatedAt,

                CategoryId,
                CategoryName,
                CategorySlug,

                VendorId,
                Brand,

                ProductCardStatus,

                CreatedAt,
                UpdatedAt,

                SeoTitle,
                SeoDescription,
                SeoKeywords,

                MainImage,

                AttributesJson,
                TagsJson,
                TagsFlat,
                ImagesJson

            FROM catalog_read.ProductCardReadModels
            WHERE Id = @Id
            """;

        using var connection = await connectionFactory.CreateConnectionAsync(
            cancellationToken);

        var command = new CommandDefinition(
            sql,
            new { Id = id },
            cancellationToken: cancellationToken);

       return await connection.QueryFirstOrDefaultAsync<ProductReadModel>(command);
    }
}
