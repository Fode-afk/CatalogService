namespace CatalogService.Domain.Abstractions;

public interface ICategoryContext
{
    bool CategoryIsActive { get; }
}