using CatalogService.Domain.Models;

namespace CatalogService.Application.Interfaces.Data;

public interface IProductCardReadRepository
{
    Task<ProductCardReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
