using CatalogService.Domain.ValueObjects;

namespace CatalogService.Domain.Abstractions;

public interface ITagsContext
{
    IReadOnlyCollection<Tag> Tags { get; }
}