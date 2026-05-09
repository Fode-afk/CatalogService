using CatalogService.Domain.Abstractions;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Contexts;

public sealed record ProductLockContext(
    bool CanBeModified,
    ProductStatus ProductStatus) : IProductContext;