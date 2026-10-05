using CatalogService.Domain.Abstractions;

namespace CatalogService.Domain.Contexts;

public sealed record ProductSuspendContext(bool CanEditContent) : IProductContext;