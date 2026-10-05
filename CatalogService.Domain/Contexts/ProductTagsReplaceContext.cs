using CatalogService.Domain.Abstractions;
using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Contexts;

public sealed record ProductTagsReplaceContext(
    bool VendorIsActive,
    bool CanEditContent,
    IReadOnlyCollection<Tag> Tags) :
        IVendorContext,
        IProductContext,
        ITagsContext;