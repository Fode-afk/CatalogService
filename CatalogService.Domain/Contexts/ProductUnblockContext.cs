using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Contexts;

public sealed record ProductUnblockContext(ProductStatus ProductStatus);