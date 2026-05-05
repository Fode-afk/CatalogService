namespace CatalogService.Domain.Contexts;

public sealed record ProductCardVendorOwnershipContext(
    Guid RequestVendorId,
    Guid ProductCardVendorId);