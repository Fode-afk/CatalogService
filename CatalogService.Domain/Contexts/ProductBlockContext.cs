using CatalogService.Domain.Abstractions;
using migApp.Shared.Enums.Products;

namespace CatalogService.Domain.Contexts;

public sealed record ProductBlockContext(
    bool CanEditContent,
    ProductStatus ProductStatus) : IProductContext;