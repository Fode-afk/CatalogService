namespace CatalogService.Domain.Abstractions;

public interface IProductContext
{
    bool CanBeModified { get; }
}
