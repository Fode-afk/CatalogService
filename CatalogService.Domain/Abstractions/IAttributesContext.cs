using CatalogService.Domain.Models;

namespace CatalogService.Domain.Abstractions;

public interface IAttributesContext
{
    IReadOnlyCollection<ProductAttribute> Attributes { get; }
}
