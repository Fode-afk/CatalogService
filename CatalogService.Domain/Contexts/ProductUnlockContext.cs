using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Contexts;

public sealed record ProductUnlockContext(ProductStatus ProductStatus);