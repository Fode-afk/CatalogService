using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Abstractions;

public interface IAttributesContext
{
    IReadOnlyCollection<ProductCardAttribute> Attributes { get; }
}
