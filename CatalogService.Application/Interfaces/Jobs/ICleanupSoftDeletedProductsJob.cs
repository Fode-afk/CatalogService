namespace CatalogService.Application.Interfaces.Jobs;

public interface ICleanupSoftDeletedProductsJob
{
    Task Execute(CancellationToken cancellationToken = default);
}