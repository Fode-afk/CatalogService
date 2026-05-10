namespace CatalogService.Application.Interfaces.Jobs;

public interface ISuspendBrandProductsJob
{
    Task Execute(Guid brandId, bool isActive, CancellationToken cancellationToken = default);
}
