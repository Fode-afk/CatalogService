namespace CatalogService.Domain.Contexts;

public sealed record ProductVendorOwnershipContext(
    Guid RequestVendorId,
    Guid ProductCardVendorId);