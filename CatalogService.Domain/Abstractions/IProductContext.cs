namespace CatalogService.Domain.Abstractions;

public interface IProductContext
{
    bool CanEditContent { get; }
}
