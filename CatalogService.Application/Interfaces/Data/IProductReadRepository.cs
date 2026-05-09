using CatalogService.Domain.Models;

namespace CatalogService.Application.Interfaces.Data;

public interface IProductReadRepository
{
    Task<ProductReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
