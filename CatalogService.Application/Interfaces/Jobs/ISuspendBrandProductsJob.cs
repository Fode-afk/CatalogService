namespace CatalogService.Application.Interfaces.Jobs;

public interface ISuspendBrandProductsJob
{
    Task Execute(Guid brandId, CancellationToken cancellationToken = default);
}
