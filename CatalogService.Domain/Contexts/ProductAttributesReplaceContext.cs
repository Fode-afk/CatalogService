using CatalogService.Domain.Abstractions;
using CatalogService.Domain.Models;

namespace CatalogService.Domain.Contexts;

public sealed record ProductAttributesReplaceContext(
    bool VendorIsActive,
    bool CanBeModified,
    IReadOnlyCollection<ProductAttribute> Attributes) :
        IVendorContext,
        IProductContext,
        IAttributesContext;